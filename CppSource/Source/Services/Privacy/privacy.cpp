// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/privacy_c.h"
#include "privacy_taskargs.h"
#include "xbox_live_context_impl.h"

using namespace xbox::services::privacy;

HC_RESULT get_privacy_list_execute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<privacy_user_list_taskargs*>(context);
    auto privacyService = args->pXboxLiveContext->pImpl->cppObject().privacy_service();

    auto result = privacyService.get_avoid_or_mute_list(args->subpathName).get();
    args->copy_xbox_live_result(result);

    if (!result.err())
    {
        for (const auto& xuid : result.payload())
        {
            args->xboxUserIds.emplace_back(utils::to_utf8string(xuid));
            args->xboxUserIdPointers.emplace_back(args->xboxUserIds.back().data());
        }
    }
    return HCTaskSetCompleted(taskHandle);
}

HC_RESULT get_privacy_list_write_results(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle,
    _In_opt_ void* completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    auto args = reinterpret_cast<privacy_user_list_taskargs*>(context);
    auto callback = reinterpret_cast<XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE>(completionRoutine);

    size_t count = args->xboxUserIdPointers.size();
    auto firstItem = count > 0 ? &args->xboxUserIdPointers[0] : nullptr;
    callback(args->result, firstItem, count, completionRoutineContext);

    delete args;
    return HC_OK;
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
PrivacyGetAvoidList(
    _In_ XSAPI_XBOX_LIVE_CONTEXT* pContext,
    _In_ XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new privacy_user_list_taskargs();
    args->pXboxLiveContext = pContext;
    args->subpathName = L"avoid";

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            get_privacy_list_execute,
            static_cast<void*>(args),
            get_privacy_list_write_results,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
PrivacyGetMuteList(
    _In_ XSAPI_XBOX_LIVE_CONTEXT* pContext,
    _In_ XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new privacy_user_list_taskargs();
    args->pXboxLiveContext = pContext;
    args->subpathName = L"mute";

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            get_privacy_list_execute,
            static_cast<void*>(args),
            get_privacy_list_write_results,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()

HC_RESULT check_permission_execute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<privacy_check_permission_taskargs*>(context);
    auto privacyService = args->pXboxLiveContext->pImpl->cppObject().privacy_service();

    auto result = privacyService.check_permission_with_target_user(
        utils::to_utf16string(args->permissionId),
        utils::to_utf16string(args->xboxUserId))
        .get();

    args->copy_xbox_live_result(result);

    if (!result.err())
    {
        args->payloadImpl.update(result.payload(), &args->completionRoutinePayload);
    }
    return HCTaskSetCompleted(taskHandle);
}


XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
PrivacyCheckPermissionWithTargetUser(
    _In_ XSAPI_XBOX_LIVE_CONTEXT* pContext,
    _In_ PCSTR permissionId,
    _In_ PCSTR xboxUserId,
    _In_ XSAPI_PRIVACY_CHECK_PERMISSION_WITH_TARGET_USER_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    // TODO here and elsewhere - switch to memory hook function instead of 'new'
    auto args = new privacy_check_permission_taskargs();
    args->pXboxLiveContext = pContext;
    args->xboxUserId = xboxUserId;
    args->permissionId = permissionId;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            check_permission_execute,
            static_cast<void*>(args),
            utils::execute_completion_routine_with_payload<privacy_check_permission_taskargs, XSAPI_PRIVACY_CHECK_PERMISSION_WITH_TARGET_USER_COMPLETION_ROUTINE>,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()

HC_RESULT check_multiple_permissions_execute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<privacy_check_multiple_permissions_taskargs*>(context);
    auto privacyService = args->pXboxLiveContext->pImpl->cppObject().privacy_service();

    auto result = privacyService.check_multiple_permissions_with_multiple_target_users(
        utils::to_string_vector(args->permissionIds, args->permissionIdsCount),
        utils::to_string_vector(args->xboxUserIds, args->xboxUserIdsCount))
        .get();

    args->copy_xbox_live_result(result);

    if (!result.err())
    {
        auto& cppPermissions = result.payload();
        args->permissions = std::vector<XSAPI_PRIVACY_MULTIPLE_PERMISSIONS_CHECK_RESULT>(cppPermissions.size());

        unsigned int index = 0;
        for (auto& cppPermission : cppPermissions)
        {
            args->permissionsImpls.emplace_back(cppPermission, &args->permissions[index++]);
        }
    }
    return HCTaskSetCompleted(taskHandle);
}

HC_RESULT check_multiple_permissions_write_results(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle,
    _In_opt_ void* completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    auto args = reinterpret_cast<privacy_check_multiple_permissions_taskargs*>(context);
    auto callback = reinterpret_cast<XSAPI_PRIVACY_CHECK_PERMISSION_WITH_MULTIPLE_TARGET_USERS_COMPLETION_ROUTINE>(completionRoutine);

    size_t count = args->permissions.size();
    auto firstItem = count > 0 ? &args->permissions[0] : nullptr;
    callback(args->result, firstItem, count, completionRoutineContext);

    delete args;
    return HC_OK;
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
PrivacyCheckMultiplePermissionsWithMultipleTargetUsers(
    _In_ XSAPI_XBOX_LIVE_CONTEXT* pContext,
    _In_ PCSTR* permissionIds,
    _In_ size_t permissionIdsCount,
    _In_ PCSTR* xboxUserIds,
    _In_ size_t xboxUserIdsCount,
    _In_ XSAPI_PRIVACY_CHECK_PERMISSION_WITH_MULTIPLE_TARGET_USERS_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new privacy_check_multiple_permissions_taskargs();
    args->pXboxLiveContext = pContext;
    args->permissionIds = permissionIds;
    args->permissionIdsCount = permissionIdsCount;
    args->xboxUserIds = xboxUserIds;
    args->xboxUserIdsCount = xboxUserIdsCount;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            check_multiple_permissions_execute,
            static_cast<void*>(args),
            check_multiple_permissions_write_results,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()
