using Braintree;
namespace BookShop.Utility.BrainTree;

public interface IBrainTreeGate
{
    IBraintreeGateway CreateGateway();
    IBraintreeGateway GetGateway();
    string GetClientToken(IBrainTreeGate brainTreeGate);
}