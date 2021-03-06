﻿using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Models.Rookery;
using FlightNode.DataCollection.Services.Models.Survey;
using FligthNode.Common.Api.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Http;

namespace FlightNode.DataCollection.Services.Controllers
{
    /// <summary>
    /// API Controller for submitting Waterbird Foraging surveys.
    /// </summary>
    public class WaterbirdForagingSurveyController : LoggingController
    {

        private const string COMPLETE = "Complete";
        private const string PENDING = "Pending";
        private const string MISSING = "missing";

        private readonly IWaterbirdForagingManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="WaterbirdForagingSurveyController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="IWorkLogDomainManager"/></param>
        public WaterbirdForagingSurveyController(IWaterbirdForagingManager domainManager)
        {
            if (domainManager == null)
            {
                throw new ArgumentNullException(nameof(domainManager));
            }

            _domainManager = domainManager;
        }

        /// <summary>
        /// Retrieves the requested Waterbird Foraging Survey data.
        /// </summary>
        /// <param name="surveyIdentifier">
        /// Unique identifier for the survey resource to retrieve.
        /// </param>
        /// <returns>
        /// <see cref="IHttpActionResult"/> containiing a <see cref="WaterbirdForagingModel"/> or status 404 if none found.
        /// </returns>
        [HttpGet]
        [Authorize]
        public IHttpActionResult Get(Guid surveyIdentifier)
        {
            return WrapWithTryCatch(() =>
            {
                var result = _domainManager.FindBySurveyId(surveyIdentifier);

                if (result == null)
                {
                    return NotFound();
                }

                var model = Map(result);

                return Ok(model);
            });
        }

        /// <summary>
        /// Retrieves a a list of Waterbird Foraging information for a given user / submitter.
        /// </summary>
        /// <param name="userId">
        /// UserId of the person who submitted the survey.
        /// </param>
        /// <returns>
        /// <see cref="IHttpActionResult"/> containing a list of <see cref="WaterbirdForagingListItem"/> or 404 if none found.
        /// </returns>
        [HttpGet]
        [Authorize]
        public IHttpActionResult Get(int userId)
        {
            return WrapWithTryCatch(() =>
            {
                var result = _domainManager.FindBySubmitterId(userId);

                if (result == null || !result.Any())
                {
                    return NotFound();
                }

                var models = result.Select(x =>
                {
                    return new WaterbirdForagingListItem
                    {
                        Location = x.LocationName ?? MISSING,
                        StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToShortDateString() : MISSING,
                        Status = (x.Step == SurveyCompleted.COMPLETED_FORAGING_STEP_NUMBER ? "Complete" : "Pending"),
                        SurveyComments = x.GeneralComments,
                        SurveyIdentifier = x.SurveyIdentifier
                    };
                });

                return Ok(models);
            });
        }


        /// <summary>
        /// Creates a new waterbird foraging survey record
        /// </summary>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody]WaterbirdForagingModel input)
        {
            if (input == null)
            {
                return BadRequest("null input");
            }

            return WrapWithTryCatch(() =>
            {
                var identifier = _domainManager.NewIdentifier();

                SurveyPending entity = Map(input, identifier);

                entity = _domainManager.Create(entity);

                var result = Map(entity);

                return Created(result, identifier.ToString());
            });
        }

        private WaterbirdForagingModel Map(ISurvey input)
        {
            var entity = new WaterbirdForagingModel
            {
                AccessPointInfoId = input.AccessPointId,
                SiteTypeId = input.AssessmentId,
                DisturbanceComments = input.DisturbanceComments,
                EndDate = input.EndDate,
                SurveyComments = input.GeneralComments,
                LocationId = input.LocationId,
                StartDate = input.StartDate,
                Temperature = input.StartTemperature,
                SurveyIdentifier = input.SurveyIdentifier,
                TideInfoId = input.TideId,
                TimeOfLowTide = input.TimeOfLowTide,
                VantagePointInfoId = input.VantagePointId,
                WeatherInfoId = input.WeatherId,
                WindSpeed = input.WindSpeed,
                SurveyId = input.Id,
                Step = input.Step
            };

            foreach (var o in input.Observations)
            {
                entity.Add(new ObservationModel
                {
                    Adults = o.Bin1,
                    Juveniles = o.Bin2,
                    BirdSpeciesId = o.BirdSpeciesId,
                    FeedingId = o.FeedingSuccessRate,
                    HabitatId = o.HabitatTypeId,
                    PrimaryActivityId = o.PrimaryActivityId,
                    SecondaryActivityId = o.SecondaryActivityId,
                    ObservationId = o.Id
                });
            }

            foreach (var d in input.Disturbances)
            {
                entity.Add(new DisturbanceModel
                {
                    DisturbanceTypeId = d.DisturbanceTypeId,
                    DurationMinutes = d.DurationMinutes,
                    Quantity = d.Quantity,
                    Behavior = d.Result,
                    DisturbanceId = d.Id
                });
            }

            foreach (var u in input.Observers)
            {
                entity.Add(u);
            }

            return entity;
        }

        /// <summary>
        /// Updates an existing new waterbird foraging survey record
        /// </summary>
        /// <param name="surveyIdentifier"></param>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/waterbirdforagingsurvey/{surveyIdentifier:Guid}")]
        [Authorize]
        public IHttpActionResult Put(Guid surveyIdentifier, [FromBody]WaterbirdForagingModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (surveyIdentifier == Guid.Empty)
            {
                return BadRequest("Invalid Survey Identifier");
            }

            return WrapWithTryCatch(() =>
            {
                SurveyPending entity = Map(input, surveyIdentifier);

                _domainManager.Update(entity, input.Step);
                WaterbirdForagingModel result;

                if (input.Step == 4)
                {
                    var newentity = (SurveyCompleted)_domainManager.FindBySurveyId(surveyIdentifier);
                    result = Map(newentity);
                }
                else
                {

                    entity = (SurveyPending)_domainManager.FindBySurveyId(surveyIdentifier);
                    result = Map(entity);
                }



                return Ok(result);
            });
        }

        private SurveyPending Map(WaterbirdForagingModel input, Guid identifier)
        {
            var entity = new SurveyPending
            {
                AccessPointId = input.AccessPointInfoId,
                AssessmentId = input.SiteTypeId,
                DisturbanceComments = input.DisturbanceComments,
                EndDate = input.EndDate,
                EndTemperature = null,
                GeneralComments = input.SurveyComments,
                LocationId = input.LocationId,
                StartDate = input.StartDate,
                StartTemperature = input.Temperature,
                SurveyIdentifier = identifier,
                TideId = input.TideInfoId,
                SurveyTypeId = SurveyType.TERN_FORAGING,
                TimeOfLowTide = input.TimeOfLowTide,
                VantagePointId = input.VantagePointInfoId,
                WeatherId = input.WeatherInfoId,
                WindSpeed = input.WindSpeed,
                SubmittedBy = this.LookupUserId(),
                Id = input.SurveyId
            };

            foreach (var o in input.Observations)
            {
                entity.Add(new Observation
                {
                    Bin1 = o.Adults,
                    Bin2 = o.Juveniles,
                    BirdSpeciesId = o.BirdSpeciesId,
                    FeedingSuccessRate = o.FeedingId,
                    HabitatTypeId = o.HabitatId,
                    PrimaryActivityId = o.PrimaryActivityId,
                    SecondaryActivityId = o.SecondaryActivityId,
                    SurveyIdentifier = identifier,
                    Id = o.ObservationId
                });
            }

            foreach (var d in input.Disturbances)
            {
                entity.Add(new Disturbance
                {
                    DisturbanceTypeId = d.DisturbanceTypeId,
                    DurationMinutes = d.DurationMinutes,
                    Quantity = d.Quantity,
                    Result = d.Behavior,
                    SurveyIdentifier = identifier,
                    Id = d.DisturbanceId
                });
            }

            foreach (var u in input.Observers)
            {
                entity.Add(u);
            }

            return entity;
        }

        private int RetrieveCurrentUserId()
        {
            return User.Identity.GetUserId<int>();
        }
    }
}
