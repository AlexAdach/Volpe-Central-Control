using System;
using System.Collections.Generic;
using VolpeCCReact.Logging;

namespace VolpeCCReact.Services
{
    public class ServiceMediator
    {
        private static readonly Lazy<ServiceMediator> lazyInstance =
            new Lazy<ServiceMediator>(() => new ServiceMediator());

        public static ServiceMediator Instance => lazyInstance.Value;

        private LinkedList<ServiceBase> services;

        public LinkedList<ServiceBase> Services => services;

        private List<string> _errorList = new List<string>();
        private List<string> _logsList = new List<string>();

        public List<string> Errors => _errorList;
        public List<string> Logs => _logsList;
        
        private ServiceMediator()
        {
            services = new LinkedList<ServiceBase>();
        }

        public void Register(ServiceBase service)
        {
            services.AddLast(service);
        }

        public void Deregister(ServiceBase service)
        {
            if(services.Contains(service))
                services.Remove(service);   
        }

        //Probably not thread safe lol. 
        public T GetService<T>(object sender) where T : ServiceBase
        {
            Log($"Service: {sender.GetType().Name} requesting {typeof(T).Name} reference.");
            
            foreach (var service in services)
            {
                if (service.GetType() == typeof(T))
                {
                    return (T)service;
                }
            }
            return null;
        }


        //Log and error handle
        public void Log(object message)
        {
            if (message is LoggingObject logmessage)
            {
                foreach (var service in services)
                {
                    if (service is ILogger logger)
                    {
                        logger.Log(logmessage.Message);
                    }
                }
                //Save the error fault to the list of faults ** probably a bad way to do this with separation of concerns. 
                if(logmessage.Level == LogLevelType.Error)
                    _errorList.Add(logmessage.Message);
                else if (logmessage.Level == LogLevelType.Debug)
                    _logsList.Add(logmessage.Message);

            }
        }
    }

}
