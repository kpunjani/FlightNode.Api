﻿using FlightNode.DataCollection.Domain.Entities;
using System.Data.Entity.Infrastructure;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface ISurveyPersistence
    {
        ICrudSet<SurveyCompleted> SurveysCompleted { get; }
        ICrudSet<SurveyPending> SurveysPending { get; }
        ICrudSet<Disturbance> Disturbances { get; }
        ICrudSet<Observation> Observations { get; }
        ICrudSet<Location> Locations { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);
    }
}
