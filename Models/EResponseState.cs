using System;

namespace Trumgu_IntegratedManageSystem.Models
{
    public enum EResponseState
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        TRUMGU_IMS_SUCCESS = 200,

        /// <summary>
        /// 未经授权
        /// </summary>
        TRUMGU_IMS_Unauthorized = 401,

        /// <summary>
        /// 参数错误
        /// </summary>
        TRUMGU_IMS_ERROR_PARAMETER = 403,

        /// <summary>
        /// 参数格式错误
        /// </summary>
        TRUMGU_IMS_ERROR_FORMAT = 406,

        /// <summary>
        /// 未找到数据
        /// </summary>
        TRUMGU_IMS_ERROR_NOT_FOUND = 404,

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        TRUMGU_IMS_ERROR_INTERNAL = 500,

        /// <summary>
        /// 保存错误
        /// </summary>
        TRUMGU_IMS_ERROR_SAVE = 510
    }
}