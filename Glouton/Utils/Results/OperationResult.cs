using Glouton.Utils.Results;

namespace Glouton.Utils.Result;

public class OperationResult : BaseResult
{
    public OperationResult() : base()
    {

    }

    public OperationResult(bool success) : base(success)
    {

    }

    public OperationResult WithSuccess()
    {
        base.IsSuccess = true;
        return this;
    }

    public OperationResult WithFailure()
    {
        base.IsSuccess = false;
        return this;
    }

    public OperationResult WithError(string message)
    {
        base.ErrorMessage = message;
        return this.WithFailure();
    }
}