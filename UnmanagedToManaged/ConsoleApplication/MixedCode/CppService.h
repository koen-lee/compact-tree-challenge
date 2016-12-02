#pragma once

#ifdef INSIDE_MANAGED_CODE
#    define DECLSPECIFIER __declspec(dllexport)
#    define EXPIMP_TEMPLATE
#else
#    define DECLSPECIFIER __declspec(dllimport)
#    define EXPIMP_TEMPLATE extern
#endif

namespace MixedCode
{
	class DECLSPECIFIER CppService
	{
	public:
		CppService();
		virtual ~CppService();

	public:
		void process(int message);

	private:
		void * m_impl;
	};
}