using Radical.CQRS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topics.Radical.Linq;

namespace Radical.CQRS.Services
{
    class DefaultAggregateStateFinder : IDefaultAggregateStateFinder<DomainContext>
    {
        public IAggregateState FindById(DomainContext session, Type stateType, AggregateQuery aggregateQuery)
        {
            var db = session.Set(stateType);
            if(aggregateQuery.Version.HasValue)
            {
                var specificState = db.Cast<IAggregateState>()
                    .Where(a => a.Id == aggregateQuery.Id && a.Version == aggregateQuery.Version)
                    .SingleOrDefault();

                return specificState;
            }

            var state = db.Find(aggregateQuery.Id);
            return (IAggregateState)state;
        }

        public IEnumerable<IAggregateState> FindById(DomainContext session, Type stateType, params AggregateQuery[] aggregateQueries)
        {
            var exp = PredicateBuilder.True<IAggregateState>();

            foreach(var query in aggregateQueries)
            {
                if(query.Version.HasValue)
                {
                    exp = exp.Or(a => a.Id == query.Id && a.Version == query.Version);
                }
                else
                {
                    exp = exp.Or(a => a.Id == query.Id);
                }
            }

            var db = session.Set(stateType).Cast<IAggregateState>();
            var results = db.Where(exp).ToArray();

            return results;
        }

        //public IAggregateState FindById(DomainContext session, Type stateType, Guid stateId)
        //{
        //    var db = session.Set(stateType);
        //    var state = db.Find(stateId);

        //    return (IAggregateState)state;
        //}

        public IEnumerable<IAggregateState> FindById(DomainContext session, Type stateType, params Guid[] stateIds)
        {
            var db = session.Set(stateType).Cast<IAggregateState>();
            var results = db.Where(state => stateIds.Contains(state.Id)).ToList();

            return results;
        }
    }
}
