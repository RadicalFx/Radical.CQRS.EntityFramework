using Radical.CQRS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topics.Radical.Linq;

namespace Radical.CQRS.Services
{
    class DefaultAggregateFinder : IDefaultAggregateFinder<DomainContext>
    {
        IEnumerable<TAggregate> IDefaultAggregateFinder<DomainContext>.FindById<TAggregate>(DomainContext session, params AggregateQuery[] aggregateQueries)
        {
            var exp = PredicateBuilder.True<TAggregate>();

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

            var db = session.Set<TAggregate>();
            var results = db.Where(exp).ToArray();

            return results;
        }

        TAggregate IDefaultAggregateFinder<DomainContext>.FindById<TAggregate>(DomainContext session, AggregateQuery aggregateQuery)
        {
            var db = session.Set<TAggregate>();
            if(aggregateQuery.Version.HasValue)
            {
                var specific = db.Where(a => a.Id == aggregateQuery.Id && a.Version == aggregateQuery.Version).SingleOrDefault();

                return specific;
            }

            var aggregate = db.Find(aggregateQuery.Id);
            return aggregate;
        }
    }
}
