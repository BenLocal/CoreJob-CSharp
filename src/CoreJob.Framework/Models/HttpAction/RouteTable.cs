using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreJob.Framework.Abstractions;

namespace CoreJob.Framework.Models.HttpAction
{
    public class RouteTable : IEnumerable<RouteParameter>
    {
        IEnumerable<RouteParameter> _routes;

        public IEnumerator<RouteParameter> GetEnumerator()
        {
            return _routes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _routes.GetEnumerator();
        }

        protected void AddRoute<T>(string name, string pattern) where T : IJobController
        {
            var routeParameter = new RouteParameter(name, pattern, typeof(T));
            if (_routes == null)
            {
                _routes = new RouteParameter[] { routeParameter };
                return;
            }

            _routes = _routes.Concat(new RouteParameter[] { routeParameter });
        }

        public RouteParameter FindByPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return null;
            }

            return _routes.FirstOrDefault(x => x.Pattern.ToLower() == pattern.ToLower());
        }
    }
}
