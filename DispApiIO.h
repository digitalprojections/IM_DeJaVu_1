//////////////////////////////////////////////////////////////////////////////////////////
//	< DispApiIO.h > DispApi �A�N�Z�X�֘A�w�b�_�[�t�@�C��
//
//	DispAPI �A�N�Z�X�֘A���`���܂�
//
//	�ŏI�X�V���t : 2013.12.20
//	�ŏI�X�V��	 : K.Tani
//
// Revision Histroy:
// [No]	[Date]		[Name]				[Description]
// 0000	2013.10.28	H.Mori				Create
// 0001	2013.12.20	K.Tani				Remove atlstr.h
//										Move #define to dispapiio_def.h
//
//				Copyright (C) 2013 Digital Electronics Corporation. All Right Reserved.
//////////////////////////////////////////////////////////////////////////////////////////
#pragma once

#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <guiddef.h>
#include <cstringt.h>

#define DISPAPIIO_API __declspec(dllexport)

/////////////////////////////////////////////////////////////////////////////////////////
//	�v���g�^�C�v�錾
/////////////////////////////////////////////////////////////////////////////////////////
DISPAPIIO_API BOOL WINAPI DispDeviceOpen( void );
DISPAPIIO_API BOOL WINAPI DispDeviceClose( void );
DISPAPIIO_API BOOL WINAPI DispResetPipe( void );
DISPAPIIO_API BOOL WINAPI DispResetDevice( void );
