namespace Glouton.Utils.Results;

public abstract class BaseResult
{
    private bool _isSuccess;
    private bool _isFailed;

    public bool IsSuccess
    {
        get => _isSuccess;
        set
        {
            _isSuccess = value;
            _isFailed = !value;
        }
    }

    public bool IsFailed
    {
        get => _isFailed;
        set
        {
            _isFailed = value;
            _isSuccess = !value;
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public string ErrorMessage { get; set; }

    public string ErrorCode { get; set; }

    protected BaseResult(bool success)
    {
        IsSuccess = success;
        this.ErrorMessage = string.Empty;
        this.ErrorCode = string.Empty;
    }

    protected BaseResult() : this(false)
    {

    }
}