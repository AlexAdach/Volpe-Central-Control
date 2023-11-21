using Crestron.SimplSharp;
using System;
using VolpeCCReact.Logging;

namespace VolpeCCReact.Services
{
    public abstract class ServiceBase : IDisposable
    {
        protected ServiceMediator _mediator;

        internal ServiceBase() 
        {
            _mediator = ServiceMediator.Instance;
            _mediator.Register(this);
        }

        protected void Log(string message)
        {
            CrestronInvoke.BeginInvoke(_mediator.Log, new LoggingObject(this, message, LogLevelType.Debug));
        }

        protected void Error(string message)
        {
            CrestronInvoke.BeginInvoke(_mediator.Log, new LoggingObject(this, message, LogLevelType.Error));
        }

        

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mediator.Deregister(this);
            }
        }

        
    }
}
