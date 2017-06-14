// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

//#pragma warning(disable: 4265)
//#pragma warning(disable: 4266)
#pragma warning(disable: 4062)

#include <string>
#include <regex>
#include <chrono>

#ifdef _WIN32
    #include <windows.h>

    #ifndef _WIN32_WINNT_WIN10
    #define _WIN32_WINNT_WIN10 0x0A00
    #endif

    #ifndef XDK_API
    #define XDK_API (WINAPI_FAMILY == WINAPI_FAMILY_TV_APP || WINAPI_FAMILY == WINAPI_FAMILY_TV_TITLE) 
    #endif

    #ifndef UWP_API
    #define UWP_API (WINAPI_FAMILY == WINAPI_FAMILY_APP && _WIN32_WINNT >= _WIN32_WINNT_WIN10)
    #endif
#endif //#ifdef _WIN32

#ifndef _WIN32
    #ifdef _In_
    #undef _In_
    #endif
    #define _In_

    #ifdef _Ret_maybenull_
    #undef _Ret_maybenull_
    #endif
    #define _Ret_maybenull_

    #ifdef _Post_writable_byte_size_
    #undef _Post_writable_byte_size_
    #endif
    #define _Post_writable_byte_size_(X)
#endif

#if defined _WIN32
  #ifdef _NO_XSAPIIMP
    #define _XSAPIIMP
    #if _MSC_VER >= 1900
        #define _XSAPIIMP_DEPRECATED __declspec(deprecated)
    #else
        #define _XSAPIIMP_DEPRECATED
    #endif
  #else
    #ifdef _XSAPIIMP_EXPORT
      #define _XSAPIIMP __declspec(dllexport)
      #define _XSAPIIMP_DEPRECATED __declspec(dllexport, deprecated)
    #else
      #define _XSAPIIMP __declspec(dllimport)
      #define _XSAPIIMP_DEPRECATED __declspec(dllimport, deprecated)
    #endif
  #endif
#else
  #if defined _NO_XSAPIIMP || __GNUC__ < 4
     #define _XSAPIIMP
     #define _XSAPIIMP_DEPRECATED __attribute__ ((deprecated))
  #else
    #define _XSAPIIMP __attribute__ ((visibility ("default")))
    #define _XSAPIIMP_DEPRECATED __attribute__ ((visibility ("default"), deprecated))
  #endif
#endif

#ifdef _WIN32
typedef wchar_t CHAR_T;
typedef LPCWSTR PCSTR_T;
typedef wchar_t char_t;
typedef std::wstring string_t;
typedef std::wstringstream stringstream_t;
typedef std::wregex regex_t;
typedef std::wsmatch smatch_t;
#else
typedef char CHAR_T;
typedef const char* PCSTR_T;
typedef char char_t;
typedef std::string string_t;
typedef std::stringstream stringstream_t;
typedef std::regex regex_t;
typedef std::smatch smatch_t;
#endif

#ifndef _T
    #ifdef _WIN32
        #define _T(x) L ## x
    #else
        #define _T(x) x
    #endif
#endif

#ifndef XSAPI_DLLEXPORT 
    #define XSAPI_DLLEXPORT __declspec(dllexport)
#else
    #define XSAPI_DLLEXPORT __declspec(dllimport)
#endif

#define XSAPI_CALL __stdcall
#define XSAPI_ASYNC_HANDLE int

#define XBOX_LIVE_NAMESPACE xbox::services

