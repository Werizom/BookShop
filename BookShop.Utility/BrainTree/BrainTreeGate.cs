using Braintree;
using Microsoft.Extensions.Options;

namespace BookShop.Utility.BrainTree;

public class BrainTreeGate : IBrainTreeGate
{
    public BrainTreeOptions BrainTreeOptions { get; set; }
    private IBraintreeGateway brainTreeGateway { get; set; }

    public BrainTreeGate(IOptions<BrainTreeOptions> options)
    {
        BrainTreeOptions = options.Value;
    }

    public IBraintreeGateway CreateGateway()
    {
        return new BraintreeGateway(BrainTreeOptions.Environment, BrainTreeOptions.MerchantId, BrainTreeOptions.PublicKey, BrainTreeOptions.PrivateKey);
    }

    public IBraintreeGateway GetGateway()
    {
        if (brainTreeGateway == null)
        {
            brainTreeGateway = CreateGateway();
        }

        return brainTreeGateway;
    }

    public string GetClientToken(IBrainTreeGate brainTreeGate)
    {
        var gateway = brainTreeGate.GetGateway();
        var clientToken = gateway.ClientToken.Generate();

        return clientToken;
    }
}
