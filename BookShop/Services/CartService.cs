using BookShop.DataAccess.Repository.IRepository;
using BookShop.Extensions;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BookShop.Services;

public class CartService : ICartService
{
    private readonly IProductRepository prodRepo;
    private readonly IInquiryHeaderRepository inquiryHeaderRepo;
    private readonly IInquiryDetailRepository inquiryDetailRepo;
    private readonly IOrderHeaderRepository orderHeaderRepo;
    private readonly IOrderDetailRepository orderDetailRepo;
    private readonly IApplicationUserRepository userRepo;
    private readonly ITransactionService transactionService;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IEmailSender emailSender;

    public CartService(IProductRepository prodRepo,
        IInquiryHeaderRepository inquiryHeaderRepo,
        IInquiryDetailRepository inquiryDetailRepo,
        IOrderHeaderRepository orderHeaderRepo,
        IOrderDetailRepository orderDetailRepo,
        IApplicationUserRepository userRepo,
        ITransactionService transactionService,
        IWebHostEnvironment webHostEnvironment,
        IEmailSender emailSender)
    {
        this.prodRepo = prodRepo;
        this.inquiryHeaderRepo = inquiryHeaderRepo;
        this.inquiryDetailRepo = inquiryDetailRepo;
        this.orderHeaderRepo = orderHeaderRepo;
        this.orderDetailRepo = orderDetailRepo;
        this.userRepo = userRepo;
        this.transactionService = transactionService;
        this.webHostEnvironment = webHostEnvironment;
        this.emailSender = emailSender;
    }

    public List<ShoppingCart> GetCartFromSession(HttpContext httpContext)
    {
        List<ShoppingCart> shoppingCart = new List<ShoppingCart>();

        if (httpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstans.SessionCart) != null
            && httpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstans.SessionCart).Count() > 0)
        {
            shoppingCart = httpContext.Session.Get<List<ShoppingCart>>(WebConstans.SessionCart);
        }

        return shoppingCart;
    }

    public ApplicationUser GetUserForRegularUser(ClaimsPrincipal user)
    {
        var claim = GetClaimIdentityUser(user);

        return userRepo.FirstOrDefault(i => i.Id == claim.Value);
    }

    public Claim GetClaimIdentityUser(ClaimsPrincipal user)
    {
        var claimsIdentity = user.Identity as ClaimsIdentity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        return claim;
    }

    public List<Product> GenerateProductList(List<ShoppingCart> shoppingCartList)
    {
        var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
        var productListTemp = prodRepo.GetAll(i => prodInCart.Contains(i.Id));
        var productList = new List<Product>();

        foreach (var cartItem in shoppingCartList)
        {
            var productTemp = productListTemp.FirstOrDefault(i => i.Id == cartItem.ProductId);
            productTemp.TempCount = cartItem.Count;
            productList.Add(productTemp);
        }

        return productList;
    }

    public ApplicationUser GetUserForAdmin(HttpContext httpContext)
    {
        if (httpContext.Session.Get<int>(WebConstans.SessionInquiryId) != 0)
        {
            var inquiryHeader = inquiryHeaderRepo.FirstOrDefault(i => i.Id == httpContext.Session.Get<int>(WebConstans.SessionInquiryId));

            return new ApplicationUser()
            {
                Email = inquiryHeader.Email,
                FullName = inquiryHeader.FullName,
                PhoneNumber = inquiryHeader.PhoneNumber,
            };
        }

        return new ApplicationUser();
    }

    public void GetListProductFromDBBasedOnCart(List<ShoppingCart> shoppingCart)
    {
        List<int> prodInCart = shoppingCart.Select(i => i.ProductId).ToList(); // на основі коризини покупок береться інфа з бд
        IEnumerable<Product> productList = prodRepo.GetAll(i => prodInCart.Contains(i.Id));
    }

    public ProductUserViewModel GetListFromCartUserViewModel(ApplicationUser applicationUser, List<ShoppingCart> shoppingCart, ProductUserViewModel productUserViewModel)
    {
        productUserViewModel = new ProductUserViewModel() // отримання списку товару отриманого з корзини покупок
        {
            ApplicationUser = applicationUser
        };

        foreach (var cartItem in shoppingCart)
        {
            Product productTemp = prodRepo.FirstOrDefault(i => i.Id == cartItem.ProductId);
            productTemp.TempCount = cartItem.Count;
            productUserViewModel.ProductList.Add(productTemp);
        }

        return productUserViewModel;
    }

    public async Task<OrderHeader> ProcessAdminOrder(IFormCollection collection, ProductUserViewModel productUserViewModel, Claim claim)
    {
        var orderHeader = CreateAndSaveOrderHeader(claim, productUserViewModel);
        AddOrderDetail(productUserViewModel, orderHeader);

        var request = transactionService.CreateTransaction(orderHeader, collection);
        var resultTransaction = transactionService.GetResultTransaction(request);

        transactionService.ChangeOrderStatus(resultTransaction, orderHeader, orderHeaderRepo);

        return orderHeader;
    }

    private OrderHeader CreateAndSaveOrderHeader(Claim claim, ProductUserViewModel productUserViewModel)
    {
        OrderHeader orderHeader = new OrderHeader()
        {
            CreatedByUserId = claim.Value,
            FinalOrderTotal = productUserViewModel.ProductList.Sum(x => x.TempCount * x.Price),
            City = productUserViewModel.ApplicationUser.City,
            StreetAddress = productUserViewModel.ApplicationUser.StreetAddress,
            State = productUserViewModel.ApplicationUser.State,
            PostalCode = productUserViewModel.ApplicationUser.PostalCode,
            FullName = productUserViewModel.ApplicationUser.FullName,
            Email = productUserViewModel.ApplicationUser.Email,
            PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
            OrderDate = DateTime.Now,
            OrderStatus = WebConstans.StatusPending
        };

        orderHeaderRepo.Add(orderHeader);
        orderHeaderRepo.Save();

        return orderHeader;
    }
    private void AddOrderDetail(ProductUserViewModel productUserViewModel, OrderHeader orderHeader)
    {
        foreach (var prod in productUserViewModel.ProductList)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                OrderHeaderId = orderHeader.Id,
                PricePerCount = prod.Price,
                Count = prod.TempCount,
                ProductId = prod.Id
            };
            orderDetailRepo.Add(orderDetail);
        }
        orderDetailRepo.Save();
    }

    public async Task ProcessUserInquiry(ProductUserViewModel productUserViewModel, Claim claim)
    {
        var PathToTemplate = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

        var subject = "New Inquiry";
        string htmlBody = "";

        using (StreamReader stream = File.OpenText(PathToTemplate))
        {
            htmlBody = stream.ReadToEnd();
        }

        var productList = GenerateProductListHtml(productUserViewModel);

        string messageBody = string.Format(htmlBody,
                productUserViewModel.ApplicationUser.FullName,
                productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber,
        productList);

        await emailSender.SendEmailAsync(WebConstans.EmailAdmin, subject, messageBody);

        var inquiryHeader = CreateAndSaveInquiryHeader(claim, productUserViewModel);
        AddInquiryDetail(productUserViewModel, inquiryHeader);
    }

    private string GenerateProductListHtml(ProductUserViewModel productUserViewModel)
    {
        var productList = new StringBuilder();
        foreach (var prod in productUserViewModel.ProductList)
        {
            productList.Append($" - Name: {prod.Name} <span style='font-size: 14px'/> (ID: {prod.Id})</span><br />");
        }
        return productList.ToString();
    }
    private InquiryHeader CreateAndSaveInquiryHeader(Claim claim, ProductUserViewModel productUserViewModel)
    {
        InquiryHeader inquiryHeader = new InquiryHeader()
        {
            ApplicationUserId = claim.Value,
            FullName = productUserViewModel.ApplicationUser.FullName,
            Email = productUserViewModel.ApplicationUser.Email,
            PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
            InquiryDate = DateTime.Now
        };

        inquiryHeaderRepo.Add(inquiryHeader);
        inquiryHeaderRepo.Save();

        return inquiryHeader;
    }
    private void AddInquiryDetail(ProductUserViewModel productUserViewModel, InquiryHeader inquiryHeader)
    {
        foreach (var prod in productUserViewModel.ProductList) // додавання в детаіл
        {
            InquiryDetail inquiryDetail = new InquiryDetail()
            {
                InquiryHeaderId = inquiryHeader.Id,
                ProductId = prod.Id,
            };
            inquiryDetailRepo.Add(inquiryDetail);
        }
        inquiryDetailRepo.Save();
    }

    public List<ShoppingCart> UpdateCountInCart(IEnumerable<Product> products, HttpContext httpContext)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        foreach (Product product in products)
        {
            shoppingCartList.Add(new ShoppingCart
            {
                ProductId = product.Id,
                Count = product.TempCount
            });
        }

        httpContext.Session.Set(WebConstans.SessionCart, shoppingCartList);

        return shoppingCartList;
    }

    public void Remove(int id, HttpContext httpContext)
    {
        var shoppingCart = GetCartFromSession(httpContext);

        shoppingCart.Remove(shoppingCart.FirstOrDefault(i => i.ProductId == id));
        httpContext.Session.Set(WebConstans.SessionCart, shoppingCart);
    }

}