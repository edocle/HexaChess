
using System;
using System.Collections.Generic;
using UnityEngine;

namespace edocle.core
{
    /// <summary>
    /// Handles all third party services
    /// - Store them
    /// - Generate them if they are not generated yet
    /// - Kill them if needed (ex: global restart of app)
    /// </summary>
    public class ThirdPartyServicesHandler
    {
        List<ThirdPartyService> m_Services = new List<ThirdPartyService>();
        ThirdPartyServiceContext m_Context = null;

        public ThirdPartyServicesHandler(GameStarterParameters parameters)
        {
            // @todo add links needed
            m_Context = new ThirdPartyServiceContext(parameters);
        }

        public T Get<T>() where T : ThirdPartyService
        {
            // Return right type of service if already generated
            foreach (var service in m_Services)
                if (service is T)
                    return (T)service;

            // Generate service
            var newService = GenerateService<T>();
            return newService;
        }

        T GenerateService<T>() where T : ThirdPartyService
        {
            var newService = (T)Activator.CreateInstance(typeof(T), m_Context);
            m_Services.Add(newService);
            return newService;
        }

        public void Kill()
        {
            foreach (var service in m_Services)
                service.Kill();

            m_Services.Clear();
        }
    }

    public abstract class ThirdPartyService
    {
        protected ThirdPartyServiceContext m_Context = null;

        protected ThirdPartyService(ThirdPartyServiceContext context)
        {
            m_Context = context;

        }

        public abstract void Kill();
    }

    /// <summary>
    /// Abstract class made to generate various third party services (TPS)
    /// ex: Save, Config, Assets, Multiplayer, etc.
    /// - A TPS is made to transmit orders & results, regardless of the tech used
    /// - A TPS need a third party actor (TPSA) to work
    /// - Each TPSA is a different tech used to compute orders & give results
    /// </summary>
    public abstract class ThirdPartyService<Actor> : ThirdPartyService where Actor : ThirdPartyServiceActor
    {
        protected List<Actor> m_Actors = new List<Actor>();


        /// <summary>
        /// Called by ThirdPartyServicesHandler.GenerateActor()
        /// </summary>
        /// <param name="context"></param>
        protected ThirdPartyService(ThirdPartyServiceContext context) : base(context)
        {
            //
        }

        public T Get<T>() where T : Actor
        {
            var actor = m_Actors.Find(f => f is T);
            if (actor != null)
                return actor as T;

            actor = GenerateActor<T>();
            return actor as T;
        }

        T GenerateActor<T>() where T : Actor
        {
            var actor = (T)Activator.CreateInstance(typeof(T), m_Context);
            m_Actors.Add(actor);
            return actor;
        }

        void TryKillActors()
        {
            if (m_Actors.Count == 0)
                return;

            foreach (var actor in m_Actors)
                actor.Kill();
        }

        public override void Kill()
        {
            TryKillActors();
        }
    }

    /// <summary>
    /// third Party Service Actor (TPSA) is a techno who is supposed to compile commands from TPS & give results
    /// ex: For a save system, can have an actor based on playerprefs or JSON
    /// - A TPSA can also be a debug actor for debug purposes (ex: send fictional callbacks)
    /// </summary>
    public abstract class ThirdPartyServiceActor
    {
        ThirdPartyServiceContext m_Context = null;

        /// <summary>
        /// Called by ThirdPartyService<T>.GenerateActor()
        /// </summary>
        /// <param name="context"></param>
        protected ThirdPartyServiceActor(ThirdPartyServiceContext context)
        {
            m_Context = context;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">All actors need their generic callback to make sur init is going well</param>
        public abstract void Init(AsyncProcessTask callback);

        public abstract void Kill();
    }

    /// <summary>
    /// Contains all links a third party service could need to work
    /// ex: System save, System config..
    /// </summary>
    public class ThirdPartyServiceContext
    {
        public SystemSaveDataHandler SystemSaveDataHandler { get; private set; }
        public List<GameSaveDataHandler> GameSaveDataHandlers { get; private set; }

        public ThirdPartyServiceContext(GameStarterParameters parameters)
        {
            SystemSaveDataHandler = parameters.SystemSaveDataHandler;
            GameSaveDataHandlers = parameters.GameSaveDataHandlers;
        }
    }
}
