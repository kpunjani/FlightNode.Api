﻿using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Controllers;
using FlightNode.DataCollection.Services.Models.Rookery;
using FlightNode.DataCollection.Services.Models.Survey;
using FligthNode.Common.Api.Controllers;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller
{
    public class WaterbirdForagingSurveyControllerTests
    {
        public class Fixture : LoggingControllerBaseFixture<WaterbirdForagingSurveyController, IWaterbirdForagingManager>
        {
            protected readonly Guid IDENTIFIER = new Guid("a507f681-c111-447a-bc1f-195916891226");
            protected const int ACCESS_POINT = 1;
            protected const string DISTURBED = "disturbed";
            protected const string DISTURBED_BEHAVIOR = "!#%#@#%";
            protected const int DISTURBED_TYPE_ID = 2;
            protected const int DISTURBED_DURATION = 234;
            protected const int DISTURBED_QUANTITY = 3;
            protected readonly DateTime END_DATE = new DateTime(2013, 2, 3, 5, 2, 6);
            protected readonly DateTime START_DATE = new DateTime(2013, 2, 3, 2, 2, 6);
            protected readonly DateTime LOW_TIDE = new DateTime(2013, 2, 3, 2, 2, 8);
            protected const int LOCATION_ID = 4;
            protected const int ADULTS = 6;
            protected const int SPECIES_ID = 7;
            protected const int FEEDING_ID = 8;
            protected const int HABITAT_ID = 9;
            protected const int JUVENILES = 10;
            protected const int PRIMARY_ACTIVITY_ID = 11;
            protected const int SECONDARY_ACTIVITY_ID = 12;
            protected const int OBSERVER_ID = 13;
            protected const int SITE_TYPE_ID = 14;
            protected const int STEP = 15;
            protected const string SURVEY_COMMENTS = "Survey comments";
            protected const int TEMPERATURE = 16;
            protected const int TIDE = 17;
            protected const int VANTAGE_POINT = 18;
            protected const int WEATHER = 19;
            protected const int WIND = 20;
            protected const int SURVEY_ID = 21;
            protected const int OBSERVATION_ID = 22;
            protected const int DISTURBANCE_ID = 23;
            protected const string LOCATION_NAME = "Charlie's Pasture";

            protected ISurvey BuildDefaultSurvey(int step = 1)
            {
                var domainResult = new SurveyPending();
                domainResult.AccessPointId = ACCESS_POINT;
                domainResult.AssessmentId = SITE_TYPE_ID;
                domainResult.DisturbanceComments = DISTURBED;
                domainResult.Disturbances.Add(new Disturbance
                {
                    Id = DISTURBANCE_ID,
                    DisturbanceTypeId = DISTURBED_TYPE_ID,
                    DurationMinutes = DISTURBED_DURATION,
                    Quantity = DISTURBED_QUANTITY,
                    Result = DISTURBED_BEHAVIOR,
                    SurveyIdentifier = IDENTIFIER
                });
                domainResult.EndDate = END_DATE;
                domainResult.GeneralComments = SURVEY_COMMENTS;
                domainResult.Id =SURVEY_ID;
                domainResult.LocationId = LOCATION_ID;
                domainResult.Observations.Add(new Observation
                {
                    Id = OBSERVATION_ID,
                    Bin1 = ADULTS,
                    Bin2 = JUVENILES,
                    BirdSpeciesId = SPECIES_ID,
                    FeedingSuccessRate = FEEDING_ID,
                    HabitatTypeId = HABITAT_ID,
                    PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                    SecondaryActivityId = SECONDARY_ACTIVITY_ID,
                    SurveyIdentifier = IDENTIFIER
                });
                domainResult.Observers.Add(OBSERVER_ID);
                domainResult.StartDate = START_DATE;
                domainResult.StartTemperature = TEMPERATURE;
                domainResult.SubmittedBy = 14;
                domainResult.SurveyIdentifier = IDENTIFIER;
                domainResult.SurveyTypeId = 15;
                domainResult.TideId =TIDE;
                domainResult.TimeOfLowTide = LOW_TIDE;
                domainResult.VantagePointId = VANTAGE_POINT;
                domainResult.WeatherId = WEATHER;
                domainResult.WindSpeed = WIND;
                domainResult.LocationName = LOCATION_NAME;
                domainResult.Step = step;
                return domainResult;
            }

            protected WaterbirdForagingModel CreateDefautInput()
            {
                var input = new WaterbirdForagingModel
                {
                    AccessPointInfoId = ACCESS_POINT,
                    DisturbanceComments = DISTURBED,
                    EndDate = END_DATE,
                    LocationId = LOCATION_ID,
                    SiteTypeId = SITE_TYPE_ID,
                    StartDate = START_DATE,
                    Step = STEP,
                    SurveyComments = SURVEY_COMMENTS,
                    Temperature = TEMPERATURE,
                    TideInfoId = TIDE,
                    VantagePointInfoId = VANTAGE_POINT,
                    WeatherInfoId = WEATHER,
                    WindSpeed = WIND,
                    TimeOfLowTide = LOW_TIDE,
                    SurveyId = SURVEY_ID
                };
                input.Disturbances.Add(new DisturbanceModel
                {
                    Behavior = DISTURBED_BEHAVIOR,
                    DisturbanceTypeId = DISTURBED_TYPE_ID,
                    DurationMinutes = DISTURBED_DURATION,
                    Quantity = DISTURBED_QUANTITY,
                    DisturbanceId = DISTURBANCE_ID
                });
                input.Observations.Add(new ObservationModel
                {
                    Adults = ADULTS,
                    BirdSpeciesId = SPECIES_ID,
                    FeedingId = FEEDING_ID,
                    HabitatId = HABITAT_ID,
                    Juveniles = JUVENILES,
                    PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                    SecondaryActivityId = SECONDARY_ACTIVITY_ID,
                    ObservationId = OBSERVATION_ID
                });
                input.Observers.Add(OBSERVER_ID);
                return input;
            }

        }

        public class Constructor : Fixture
        {
            [Fact]
            public void HappyPath()
            {
                BuildSystem();
            }

            [Fact]
            public void RejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new WaterbirdForagingSurveyController(null));
            }
        }

        public class GetSpecificItem : Fixture
        {
            [Fact]
            public void MapsAccessPointId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.AccessPointId, result.AccessPointInfoId);
            }

            [Fact]
            public void MapsAssessmentId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.AssessmentId, result.SiteTypeId);
            }

            [Fact]
            public void MapsDisturbanceComments()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.DisturbanceComments, result.DisturbanceComments);
            }

            [Fact]
            public void MapsDisturbance_Id()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Disturbances.First().Id, result.Disturbances.First().DisturbanceId);
            }

            [Fact]
            public void MapsDisturbance_DisturbanceTypeId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Disturbances.First().DisturbanceTypeId, result.Disturbances.First().DisturbanceTypeId);
            }

            [Fact]
            public void MapsDisturbance_Result()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Disturbances.First().Result, result.Disturbances.First().Behavior);
            }


            [Fact]
            public void MapsDisturbance_DurationMinutes()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Disturbances.First().DurationMinutes, result.Disturbances.First().DurationMinutes);
            }

            [Fact]
            public void MapsDisturbance_Quantity()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Disturbances.First().Quantity, result.Disturbances.First().Quantity);
            }

            [Fact]
            public void MapsEndDate()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.EndDate, result.EndDate);
            }

            [Fact]
            public void MapsLocationId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.LocationId, result.LocationId);
            }

            [Fact]
            public void MapsObservation_Adults()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().Bin1, result.Observations.First().Adults);
            }

            [Fact]
            public void MapsObservation_Juveniles()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().Bin2, result.Observations.First().Juveniles);
            }

            [Fact]
            public void MapsObservation_BirdSpeciesId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().BirdSpeciesId, result.Observations.First().BirdSpeciesId);
            }

            [Fact]
            public void MapsObservation_FeedingId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().FeedingSuccessRate, result.Observations.First().FeedingId);
            }



            [Fact]
            public void MapsObservation_HabitatId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().HabitatTypeId, result.Observations.First().HabitatId);
            }

            [Fact]
            public void MapsObservation_ObservationId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().Id, result.Observations.First().ObservationId);
            }

            [Fact]
            public void MapsObservation_PrimaryActivityId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().PrimaryActivityId, result.Observations.First().PrimaryActivityId);
            }

            [Fact]
            public void MapsObservation_SecondaryActivityId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observations.First().SecondaryActivityId, result.Observations.First().SecondaryActivityId);
            }

            [Fact]
            public void MapsObservers()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Observers.First(), result.Observers.First());
            }

            [Fact]
            public void MapsStartDate()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.StartDate, result.StartDate);
            }

            [Fact]
            public void MapsStep()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Step, result.Step);
            }

            [Fact]
            public void MapsSurveyComments()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.GeneralComments, result.SurveyComments);
            }

            [Fact]
            public void MapsSurveyId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.Id, result.SurveyId);
            }

            [Fact]
            public void MapsSurveyIdentifier()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.SurveyIdentifier, result.SurveyIdentifier);
            }

            [Fact]
            public void MapsTemperature()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.StartTemperature, result.Temperature);
            }

            [Fact]
            public void MapsTideInfoId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.TideId, result.TideInfoId);
            }

            [Fact]
            public void MapsTimeOfLowTide()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.TimeOfLowTide, result.TimeOfLowTide);
            }

            [Fact]
            public void MapsVantagePointInfoId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.VantagePointId, result.VantagePointInfoId);
            }

            [Fact]
            public void MapsWeatherInfoId()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.WeatherId, result.WeatherInfoId);
            }

            [Fact]
            public void MapsWindSpeed()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.WindSpeed, result.WindSpeed);
            }

            private WaterbirdForagingModel RunHappyPath(ISurvey domain)
            {
                SetupMockResult(domain);

                var result = RunTest()
                                .Content
                                .ReadAsAsync<WaterbirdForagingModel>()
                                .Result;
                return result;
            }

            [Fact]
            public void TreatsExceptionsAsServerError()
            {

                MockDomainManager.Setup(x => x.FindBySurveyId(It.Is<Guid>(y => y == IDENTIFIER)))
                    .Throws<InvalidOperationException>();

                MockLogger.Setup(x => x.Error(It.IsAny<InvalidOperationException>()));

                var result = RunTest();

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            }

            [Fact]
            public void MatchingItemGeneratesStatus200()
            {
                SetupMockResult(BuildDefaultSurvey());

                var result = RunTest();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Fact]
            public void NoMatchGenerates404()
            {
                //
                // Arrange
                SetupMockResult(null);

                //
                // Act
                var result = RunTest();

                //
                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            }

            private void SetupMockResult(ISurvey domainResult)
            {
                MockDomainManager.Setup(x => x.FindBySurveyId(It.Is<Guid>(y => y == IDENTIFIER)))
                    .Returns(domainResult);
            }

            private HttpResponseMessage RunTest()
            {
                var system = BuildSystem();

                system.Logger = MockLogger.Object;

                return system.Get(IDENTIFIER).ExecuteAsync(new System.Threading.CancellationToken()).Result;
            }
        }


        public class GetUsersList : Fixture
        {
            [Fact]
            public void MapsLocationName()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.LocationName, result.First().Location);
            }
            
            [Fact]
            public void MapsStartDate()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(START_DATE.ToShortDateString(), result.First().StartDate);
            }

            [Fact]
            public void MapsStatusForPendingSurvey()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal("Pending", result.First().Status);
            }

            [Fact]
            public void MapsStatusForCompletedSurvey()
            {
                // fake a completed survey by setting the step to 4
                var domain = BuildDefaultSurvey(SurveyCompleted.COMPLETED_FORAGING_STEP_NUMBER);

                var result = RunHappyPath(domain);

                Assert.Equal("Complete", result.First().Status);
            }

            [Fact]
            public void MapsSurveyComments()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.GeneralComments, result.First().SurveyComments);
            }

            [Fact]
            public void MapsSurveyIdentifier()
            {
                var domain = BuildDefaultSurvey();

                var result = RunHappyPath(domain);

                Assert.Equal(domain.SurveyIdentifier, result.First().SurveyIdentifier);
            }

            private IList<WaterbirdForagingListItem> RunHappyPath(ISurvey domain)
            {
                SetupMockResult(domain);

                var result = RunTest()
                                .Content
                                .ReadAsAsync<IList<WaterbirdForagingListItem>>()
                                .Result;
                return result;
            }

            [Fact]
            public void TreatsExceptionsAsServerError()
            {
                MockDomainManager.Setup(x => x.FindBySubmitterId(It.Is<int>(y => y == OBSERVER_ID)))
                    .Throws<InvalidOperationException>();

                MockLogger.Setup(x => x.Error(It.IsAny<InvalidOperationException>()));

                var result = RunTest();

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            }

            [Fact]
            public void MatchingItemGeneratesStatus200()
            {
                SetupMockResult(BuildDefaultSurvey());

                var result = RunTest();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Fact]
            public void NoMatchGenerates404()
            {
                MockDomainManager.Setup(x => x.FindBySubmitterId(It.Is<int>(y => y == OBSERVER_ID)))
                     .Returns(new List<ISurvey>());

                var result = RunTest();

                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            }

            [Fact]
            public void NullMatchGenerates404()
            {
                MockDomainManager.Setup(x => x.FindBySubmitterId(It.Is<int>(y => y == OBSERVER_ID)))
                     .Returns(null as List<ISurvey>);

                var result = RunTest();

                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            }

            private void SetupMockResult(ISurvey domainResult)
            {
                MockDomainManager.Setup(x => x.FindBySubmitterId(It.Is<int>(y => y == OBSERVER_ID)))
                    .Returns(new[] { domainResult });
            }

            private HttpResponseMessage RunTest()
            {
                var system = BuildSystem();

                system.Logger = MockLogger.Object;

                return system.Get(OBSERVER_ID).ExecuteAsync(new System.Threading.CancellationToken()).Result;
            }
        }

        public class Post : Fixture
        {

            public class HappyPath : Post
            {
                [Fact]
                public void MapsAccessPoint()
                {
                    RunPositiveTest();

                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => ACCESS_POINT == y.AccessPointId)));
                }

                [Fact]
                public void MapsDisturbanceComment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED == y.DisturbanceComments)));
                }


                [Fact]
                public void MapsEndDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => END_DATE == y.EndDate)));
                }

                [Fact]
                public void MapsStartDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => START_DATE == y.StartDate)));
                }

                [Fact]
                public void MapsLocationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => LOCATION_ID == y.LocationId)));
                }

                [Fact]
                public void MapsLowTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => LOW_TIDE == y.TimeOfLowTide)));
                }

                [Fact]
                public void MapsSiteTypeIdToAssessment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SITE_TYPE_ID == y.AssessmentId)));
                }

                [Fact]
                public void MapsSurveyCommentsToGeneralComments()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SURVEY_COMMENTS == y.GeneralComments)));
                }

                [Fact]
                public void MapsTemperature()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => TEMPERATURE == y.StartTemperature)));
                }

                [Fact]
                public void MapsTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => TIDE == y.TideId)));
                }

                [Fact]
                public void MapsVantagePoint()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => VANTAGE_POINT == y.VantagePointId)));
                }

                [Fact]
                public void MapsWeather()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => WEATHER == y.WeatherId)));
                }

                [Fact]
                public void MapsWindSpeed()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => WIND == y.WindSpeed)));
                }

                [Fact]
                public void MapsDisturbedBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_BEHAVIOR == y.Disturbances.First().Result)));
                }

                [Fact]
                public void MapsDisturbanceTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_TYPE_ID == y.Disturbances.First().DisturbanceTypeId)));
                }

                [Fact]
                public void MapsDisturbanceId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBANCE_ID == y.Disturbances.First().Id)));
                }

                [Fact]
                public void MapsDisturbanceDuration()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_DURATION == y.Disturbances.First().DurationMinutes)));
                }

                [Fact]
                public void MapsDisturbanceQuantity()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_QUANTITY == y.Disturbances.First().Quantity)));
                }

                [Fact]
                public void MapsIdentifierIntoDisturbance()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => IDENTIFIER == y.Disturbances.First().SurveyIdentifier)));
                }

                [Fact]
                public void MapsObserver()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => OBSERVER_ID == y.Observers.First())));
                }

                [Fact]
                public void MapsIdentifierIntoObservation()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => IDENTIFIER == y.Observations.First().SurveyIdentifier)));
                }

                [Fact]
                public void MapsObservationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => OBSERVATION_ID == y.Observations.First().Id)));
                }

                [Fact]
                public void MapAdultsToBin1()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => ADULTS == y.Observations.First().Bin1)));
                }


                [Fact]
                public void MapJuvenilesToBin2()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => JUVENILES == y.Observations.First().Bin2)));
                }

                [Fact]
                public void MapPrimaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => PRIMARY_ACTIVITY_ID == y.Observations.First().PrimaryActivityId)));
                }

                [Fact]
                public void MapSecondaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SECONDARY_ACTIVITY_ID == y.Observations.First().SecondaryActivityId)));
                }

                [Fact]
                public void MapSpeciesId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SPECIES_ID == y.Observations.First().BirdSpeciesId)));
                }

                [Fact]
                public void MapsFeedingId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => FEEDING_ID == y.Observations.First().FeedingSuccessRate)));
                }

                [Fact]
                public void MapsHabitatId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => HABITAT_ID == y.Observations.First().HabitatTypeId)));
                }

                [Fact]
                public void MapsSurveyTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SurveyType.TERN_FORAGING == y.SurveyTypeId)));
                }



                public override void Dispose()
                {
                    // Restore delegate extension method to default behavior
                    ExtensionDelegate.Init();

                    base.Dispose();
                }

                [Fact]
                public void MapsCurrentUserAsSubmittedBy()
                {
                    // Arrange

                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);

                    // mock the lookup of userid
                    const int userId = 13;
                    ExtensionDelegate.LookupUserIdDelegate = (LoggingController c) =>
                    {
                        return userId;
                    };


                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Callback((SurveyPending actual) => Assert.Equal(userId, actual.SubmittedBy))
                        .Returns((SurveyPending actual) => actual);

                    var system = BuildSystem();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Post(new WaterbirdForagingModel()));

                    // no asserts necessary
                }


                [Fact]
                public void RespondsWithCreated()
                {
                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                }

                [Fact]
                public void RespondsWithLocation()
                {
                    var expected = url + IDENTIFIER.ToString();

                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(expected, result.Headers.Location.ToString());
                }

                private HttpResponseMessage RunPositiveTest()
                {
                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();


                    JObject obj = JObject.FromObject(input);
                    var a = obj.ToString();

                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);

                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Returns((SurveyPending actual) => actual);


                    var system = BuildSystem();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Post(input));
                    return result;
                }


            }

            public class ExceptionHandling : Post
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);
                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Throws(ex);


                    return BuildSystem().Post(CreateDefautInput()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    ExpectToLogToError();

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    ExpectToLogToError();

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    ExpectToLogToDebug();

                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }

                [Fact]
                public void NullInputShouldBeTreatedAsABadRequest()
                {

                    var result = BuildSystem().Post(null).ExecuteAsync(new System.Threading.CancellationToken()).Result;

                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                }
            }
        }

        public class Put : Fixture
        {

            public class HappyPath : Put
            {

                public override void Dispose()
                {
                    // Restore delegate extension method to default behavior
                    ExtensionDelegate.Init();

                    base.Dispose();
                }

                [Fact]
                public void MapsCurrentUserAsSubmittedBy()
                {
                    // Arrange
                    const int userId = 23423;

                    // mock the lookup of userid
                    ExtensionDelegate.LookupUserIdDelegate = (LoggingController c) =>
                    {
                        return userId;
                    };


                    MockDomainManager.Setup(x => x.Update(It.IsAny<SurveyPending>(), It.IsAny<int>()))
                        .Callback((SurveyPending actual, int astep) =>
                        {
                            Assert.Equal(userId, actual.SubmittedBy);
                        });

                    var system = BuildSystem();


                    WaterbirdForagingModel input = CreateDefautInput();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Put(IDENTIFIER, input));

                    // no more asserts required
                }

                [Fact]
                public void MapsAccessPoint()
                {
                    RunPositiveTest();

                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => ACCESS_POINT == y.AccessPointId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceComment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED == y.DisturbanceComments), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void MapsEndDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => END_DATE == y.EndDate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsStartDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => START_DATE == y.StartDate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsLocationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => LOCATION_ID == y.LocationId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsLowTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => LOW_TIDE == y.TimeOfLowTide), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSiteTypeIdToAssessment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SITE_TYPE_ID == y.AssessmentId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSurveyCommentsToGeneralComments()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SURVEY_COMMENTS == y.GeneralComments), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsTemperature()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => TEMPERATURE == y.StartTemperature), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => TIDE == y.TideId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsVantagePoint()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => VANTAGE_POINT == y.VantagePointId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsWeather()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => WEATHER == y.WeatherId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsWindSpeed()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => WIND == y.WindSpeed), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbedBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_BEHAVIOR == y.Disturbances.First().Result), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_TYPE_ID == y.Disturbances.First().DisturbanceTypeId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBANCE_ID == y.Disturbances.First().Id), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceDuration()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_DURATION == y.Disturbances.First().DurationMinutes), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceQuantity()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_QUANTITY == y.Disturbances.First().Quantity), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsIdentifierIntoDisturbance()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => IDENTIFIER == y.Disturbances.First().SurveyIdentifier), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsObserver()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => OBSERVER_ID == y.Observers.First()), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsIdentifierIntoObservation()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => IDENTIFIER == y.Observations.First().SurveyIdentifier), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsObservationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => OBSERVATION_ID == y.Observations.First().Id), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapAdultsToBin1()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => ADULTS == y.Observations.First().Bin1), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void MapJuvenilesToBin2()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => JUVENILES == y.Observations.First().Bin2), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapPrimaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => PRIMARY_ACTIVITY_ID == y.Observations.First().PrimaryActivityId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapSecondaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SECONDARY_ACTIVITY_ID == y.Observations.First().SecondaryActivityId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapSpeciesId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SPECIES_ID == y.Observations.First().BirdSpeciesId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsFeedingId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => FEEDING_ID == y.Observations.First().FeedingSuccessRate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsHabitatId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => HABITAT_ID == y.Observations.First().HabitatTypeId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSurveyTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SurveyType.TERN_FORAGING == y.SurveyTypeId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSurveyd()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SURVEY_ID == y.Id), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void RespondsWithNoContent()
                {
                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                }


                private HttpResponseMessage RunPositiveTest()
                {
                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();
                    input.SurveyIdentifier = IDENTIFIER;

                    MockDomainManager.Setup(x => x.Update(It.IsAny<SurveyPending>(), It.Is<int>(y => y == STEP)));


                    var system = BuildSystem();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Put(IDENTIFIER, input));

                    return result;
                }


            }

            public class ExceptionHandling : Put
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Update(It.IsAny<SurveyPending>(), It.Is<int>(y => y == STEP)))
                        .Throws(ex);


                    var input = CreateDefautInput();
                    input.SurveyIdentifier = IDENTIFIER;

                    return BuildSystem().Put(IDENTIFIER, input).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void EmptyGuidIdentifierGeneratesBadRequest()
                {

                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();

                    //
                    // Act
                    var result = ExecuteHttpAction(BuildSystem().Put(Guid.Empty, input));

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    ExpectToLogToError();

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    ExpectToLogToError();

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    ExpectToLogToDebug();

                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }
        }

    }
}
