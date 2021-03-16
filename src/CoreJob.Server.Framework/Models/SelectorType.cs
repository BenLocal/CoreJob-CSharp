using CoreJob.Framework.Abstractions;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Services.Selector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Models
{
    public class SelectorType : Enumeration
    {
        /// <summary>
        /// 0, 默认第一个
        /// </summary>
        public static readonly SelectorType Default = new SelectorType(0, "默认第一个", () => new DefaultRegistryHostSelector());

        /// <summary>
        /// 1, 随机
        /// </summary>
        public static readonly SelectorType Random = new SelectorType(1, "随机", () => new RandomRegistryHostSelector());

        /// <summary>
        /// 2, 负载均衡
        /// </summary>
        public static readonly SelectorType LoadBalance = new SelectorType(2, "负载均衡", () => new LoadBalanceRegistryHostSelector());

        protected SelectorType(int value, string displayName, Func<IRegistryHostSelector> action) : base(value, displayName)
        {
            _selector = action?.Invoke();
        }

        private readonly IRegistryHostSelector _selector;
        public IRegistryHostSelector Selector => _selector;
    }
}
