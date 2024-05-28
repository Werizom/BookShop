using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Services.Interfaces;
using BookShop.Utility.BrainTree;
using BookShop.Utility.Constans;
using Braintree;

namespace BookShop.Services;

public class TransactionService : ITransactionService
{
    private readonly IBrainTreeGate brainGate;

    public TransactionService(IBrainTreeGate brainGate)
    {
        this.brainGate = brainGate;
    }

    public TransactionRequest CreateTransaction(OrderHeader orderHeader, IFormCollection collection)
    {
        string nonceFromTheClient = collection["payment_method_nonce"]; // отримання з колекції токена
        var request = new TransactionRequest                            //Create a transaction
        {
            Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
            PaymentMethodNonce = nonceFromTheClient,
            OrderId = orderHeader.Id.ToString(),
            Options = new TransactionOptionsRequest
            {
                SubmitForSettlement = true
            }
        };

        return request;
    }

    public Result<Transaction> GetResultTransaction(TransactionRequest request)
    {
        var gateway = brainGate.GetGateway();
        Result<Transaction> result = gateway.Transaction.Sale(request);

        return result;
    }

    public void ChangeOrderStatus(Result<Transaction> resultTransaction, OrderHeader orderHeader, IOrderHeaderRepository orderHeaderRepo)
    {
        if (resultTransaction.Target.ProcessorResponseText == WebConstans.StatusApproved) // зміна статусу замовлення
        {
            orderHeader.TransactionId = resultTransaction.Target.Id;
            orderHeader.OrderStatus = WebConstans.StatusApproved;
        }
        else
        {
            orderHeader.OrderStatus = WebConstans.StatusCancelled;
        }
        orderHeaderRepo.Save();
    }

}