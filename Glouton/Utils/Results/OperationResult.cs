using Glouton.Utils.Results;
using System;

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

    public static OperationResult Success => new OperationResult().WithSuccess();

    public OperationResult WithFailure()
    {
        base.IsSuccess = false;
        return this;
    }

    public static OperationResult Failure => new OperationResult().WithFailure();

    public OperationResult WithError(string message)
    {
        base.ErrorMessage = message;
        return this.WithFailure();
    }

    public static OperationResult Error(string message)
    {
        return new OperationResult().WithError(message);
    }

    public void Affect(OperationResult result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        this.IsSuccess = result.IsSuccess;
        this.ErrorMessage = result.ErrorMessage;
        this.ErrorCode = result.ErrorCode;
    }
}