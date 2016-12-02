#include "stdafx.h"
#include "CppService.h"

using namespace ClassLibrary;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace MixedCode
{
	CppService::CppService()
	{
		// Instantiate the .Net opbject
		Service^ service = gcnew Service();

		// Pin the .NET object, and record the address of the pinned object in m_impl
		m_impl = GCHandle::ToIntPtr(GCHandle::Alloc(service)).ToPointer();
	}

	CppService::~CppService()
	{
		// Get the GCHandle associated with the pinned object based on its 
		// address, and free the GCHandle. At this point, the C#
		// object is eligible for GC.
		GCHandle handle = GCHandle::FromIntPtr(IntPtr(m_impl));
		handle.Free();
	}

	void CppService::process(int message)
	{
		// Get the pinned .Net object from its memory address
		GCHandle handle = GCHandle::FromIntPtr(IntPtr(m_impl));
		Service^ service = safe_cast<Service^>(handle.Target);

		service->Process(message);
	}
}