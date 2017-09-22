// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

std::string utf8_from_utf16(_In_reads_(size) PCWSTR utf16, size_t size)
{
    // early out on empty strings since they are trivially convertible
    if (size == 0)
    {
        return "";
    }

    // query for the buffer size
    auto queryResult = WideCharToMultiByte(
        CP_UTF8, WC_ERR_INVALID_CHARS,
        utf16, static_cast<int>(size),
        nullptr, 0,
        nullptr, nullptr
    );
    if (queryResult == 0)
    {
        throw std::exception("utf8_from_utf16 failed");
    }

    // allocate the output buffer, queryResult is the required size
    std::string utf8(static_cast<size_t>(queryResult), L'\0');
    auto conversionResult = WideCharToMultiByte(
        CP_UTF8, WC_ERR_INVALID_CHARS,
        utf16, static_cast<int>(size),
        &utf8[0], static_cast<int>(utf8.size()),
        nullptr, nullptr
    );
    if (conversionResult == 0)
    {
        throw std::exception("utf8_from_utf16 failed");
    }

    return utf8;
}

std::wstring utf16_from_utf8(_In_reads_(size) PCSTR utf8, size_t size)
{
    // early out on empty strings since they are trivially convertible
    if (size == 0)
    {
        return L"";
    }

    // query for the buffer size
    auto queryResult = MultiByteToWideChar(
        CP_UTF8, MB_ERR_INVALID_CHARS,
        utf8, static_cast<int>(size),
        nullptr, 0
    );
    if (queryResult == 0)
    {
        throw std::exception("utf16_from_utf8 failed");
    }

    // allocate the output buffer, queryResult is the required size
    std::wstring utf16(static_cast<size_t>(queryResult), L'\0');
    auto conversionResult = MultiByteToWideChar(
        CP_UTF8, MB_ERR_INVALID_CHARS,
        utf8, static_cast<int>(size),
        &utf16[0], static_cast<int>(utf16.size())
    );
    if (conversionResult == 0)
    {
        throw std::exception("utf16_from_utf8 failed");
    }

    return utf16;
}

std::string utils::to_utf8string(const std::wstring& utf16)
{
    return utf8_from_utf16(utf16.data(), utf16.size());
}

std::wstring utils::to_utf16string(const std::string& utf8)
{
    return utf16_from_utf8(utf8.data(), utf8.size());
}

XSAPI_RESULT utils::std_bad_alloc_to_result(std::bad_alloc const& e, _In_z_ char const* file, uint32_t line)
{
    HC_TRACE_ERROR(XSAPI_C_TRACE, "[%d] std::bad_alloc reached api boundary: %s\n    %s:%u",
        XSAPI_E_OUTOFMEMORY, e.what(), file, line);
    return XSAPI_E_OUTOFMEMORY;
}

XSAPI_RESULT utils::std_exception_to_result(std::exception const& e, _In_z_ char const* file, uint32_t line)
{
    HC_TRACE_ERROR(XSAPI_C_TRACE, "[%d] std::exception reached api boundary: %s\n    %s:%u",
        XSAPI_E_FAIL, e.what(), file, line);

    assert(false);
    return XSAPI_E_FAIL;
}

XSAPI_RESULT utils::unknown_exception_to_result(_In_z_ char const* file, uint32_t line)
{
    HC_TRACE_ERROR(XSAPI_C_TRACE, "[%d] unknown exception reached api boundary\n    %s:%u",
        XSAPI_E_FAIL, file, line);

    assert(false);
    return XSAPI_E_FAIL;
}

XSAPI_RESULT utils::xsapi_result_from_hc_result(HC_RESULT hcr)
{
    // TODO make this is a bit more robust
    return static_cast<XSAPI_RESULT>(hcr);
}