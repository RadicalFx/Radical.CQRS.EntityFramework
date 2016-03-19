using Radical.CQRS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Radical.CQRS.Services
{
    class DefaultAggregateStateFinder : IDefaultAggregateStateFinder<DomainContext>
    {
        public IAggregateState FindById(DomainContext session, Type stateType, AggregateQuery aggregateQuery)
        {
            var db = session.Set(stateType);
            if(aggregateQuery.Version.HasValue)
            {
                var specificState = db.Where( "Id = @0 AND Version = @1", aggregateQuery.Id, aggregateQuery.Version)
                    .OfType<IAggregateState>()
                    .SingleOrDefault();

                return specificState;
            }

            var state = db.Find(aggregateQuery.Id);
            return (IAggregateState)state;
        }

        public IEnumerable<IAggregateState> FindById(DomainContext session, Type stateType, params AggregateQuery[] aggregateQueries)
        {
            var querySegments = new List<string>();
            var parameters = new List<object>();
            var counter = -1;

            foreach(var query in aggregateQueries)
            {
                if(query.Version.HasValue)
                {
                    querySegments.Add($"(Id = @{++counter} AND Version = @{++counter})");
                    parameters.Add(query.Id);
                    parameters.Add(query.Version.Value);
                }
                else
                {
                    querySegments.Add($"(Id = @{++counter})");
                    parameters.Add(query.Id);
                }
            }

            var queryText = string.Join( " OR ", querySegments );

            var db = session.Set(stateType);
            var results = db.Where(queryText).OfType<IAggregateState>();

            return results;
        }

        //public IAggregateState FindById(DomainContext session, Type stateType, Guid stateId)
        //{
        //    var db = session.Set(stateType);
        //    var state = db.Find(stateId);

        //    return (IAggregateState)state;
        //}

        //public IEnumerable<IAggregateState> FindById(DomainContext session, Type stateType, params Guid[] stateIds)
        //{
        //    var db = session.Set(stateType);

        //    var results = db.Where("Id.Contains(@0)", stateIds)
        //        .OfType<IAggregateState>();

        //    return results;
        //}
    }
}
