using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using Braintree;

namespace BookShop.Services.Interfaces;

public interface ITransactionService
{
    TransactionRequest CreateTransaction(OrderHeader orderHeader, IFormCollection collection);
    Result<Transaction> GetResultTransaction(TransactionRequest request);
    void ChangeOrderStatus(Result<Transaction> resultTransaction, OrderHeader orderHeader, IOrderHeaderRepository orderHeaderRepo);
}