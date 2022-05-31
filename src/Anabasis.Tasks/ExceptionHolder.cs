using System.Runtime.ExceptionServices;

namespace Anabasis.Tasks;

internal class ExceptionHolder
{
    ExceptionDispatchInfo exception;
    bool                  calledGet = false;

    public ExceptionHolder(ExceptionDispatchInfo exception)
    {
        this.exception = exception;
    }

    public ExceptionDispatchInfo GetException()
    {
        if (!calledGet)
        {
            calledGet = true;
            GC.SuppressFinalize(this);
        }
        return exception;
    }

    ~ExceptionHolder()
    {
        if (!calledGet)
        {
            AnabasisTaskScheduler.PublishUnobservedTaskException(exception.SourceException);
        }
    }
}