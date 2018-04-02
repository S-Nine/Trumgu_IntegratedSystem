using System;
using System.Collections.Generic;

namespace Trumgu_IntegratedManageSystem.Models {
    public class TreeDataObj {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 显示节点文本
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 节点状态
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool? check { get; set; }

        /// <summary>
        /// 被添加到节点的自定义属性
        /// </summary>
        public string attributes { get; set; }

        /// <summary>
        /// 一个节点数组声明了若干节点。
        /// </summary>
        public List<TreeDataObj> children { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public string iconCls { get; set; }
    }
}