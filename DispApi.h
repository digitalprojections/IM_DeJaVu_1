//////////////////////////////////////////////////////////////////////////////////////////
//	< DispApi.h > DispApi�w�b�_�[�t�@�C��
//
//	DispAPI���`���܂�
//
//	�ŏI�X�V���t	: 2013.10.31
//	�ŏI�X�V��		: H.Mori
//
//				Copyright (C) 2013 Digital Electronics Corporation. All Right Reserved.
//////////////////////////////////////////////////////////////////////////////////////////

#pragma once

#ifdef DISPAPI_EXPORTS
#define DISP_API __declspec(dllexport)
#else
#define DISP_API __declspec(dllimport)
#endif

#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <guiddef.h>
#include <DispApiIO.h>

#ifdef  __cplusplus
extern "C" {
#endif

/////////////////////////////////////////////////////////////////////////////////////////
//	�萔�A�v���g�^�C�v�錾
/////////////////////////////////////////////////////////////////////////////////////////
#define	DISPAPI_H_

// USB DEVICE
#define DISP_ALIVECHECK				0x30	// AliveCheck
#define DISP_PROGRAM_VERSION		0x30	// ProgramVersion
#define DISP_EEPROM_LOCAL			0x34	// EEPROM Slave A0 (into the Display unit)
//-----------------------------------------------------------------------------------------------------------------
//USBcommand�̎d�l����EEPROM�A�N�Z�X��SelfCheck�ŁABOI-DISP EEPROM��BOI-BOX EEPROM�̃f�o�C�XID��
//�H������Ă���A�d�l���ɍ��킹�邽�߂ɁA���L�̃p�^�[���̃f�o�C�XID�̒�`���s���Ă���B
#define DISP_EEPROM_BOI				0x37	// EEPROM Slave A0 (BOI-DISP�@EEPROM)
#define DISP_EEPROM_BOX				0x38	// EEPROM Slave A8
#define DISP_EEPROM_BOI_SELFCHK		0x38	// EEPROM Slave A0 (BOI-DISP�@EEPROM)	SelfCheck�̂݁A����ID���g�p����
#define DISP_EEPROM_BOX_SELFCHK		0x37	// EEPROM Slave A8						SelfCheck�̂݁A����ID���g�p����
//-----------------------------------------------------------------------------------------------------------------
#define DISP_FIRM					0x39	// Firm

//	Parameter
//  �@�@LED Color�̒l��0����n�܂��Ă��Ȃ��̂�ZenzaiDisplay��FarmWare�d�l�Ɠ���������Ă����
#define ALIVECHECK_TIMEOUT_MAX		0xffff	// AliveCheck Timeout MAX

#define PROGRAM_VERSION_LENGTH		12		// �t�@�[���E�F�A�̃o�[�W�����̒���

// �߂�l
#define RTN_SUCCESS					0		// ����I��
#define RTN_CONTINUOS				1		// �����p��(WiFi�̎��ɖ������v�����v�g������ł�ACK/NAK�ł��Ȃ��ꍇ)
#define RTN_ACK						2		// ACK���Ԃ��Ă���(����)
#define RTN_NAK						3		// NAK���Ԃ��Ă���(�G���[)
#define RTN_NOTPREPARED				4		// ����������
#define RTN_ERROR					-1		// �G���[����

// Error code list
#define	DISPAPI_ERROR_HANDLE			0xE0210100	// Error of handle.
#define	DISPAPI_ERROR_INVALID_PARAMETER	0xE0210101	// Error of index parameter.
#define	DISPAPI_ERROR_CMDSEARCH			0xE0210102	// Error of command search.
#define	DISPAPI_ERROR_CRITICALSECTION	0xE0210200	// Error of critical section.
#define	DISPAPI_ERROR_MUTEX				0xE0210201	// Error of get mutex object.
#define	DISPAPI_ERROR_ATTACH			0xE0210202	// Error of attach the HID.
#define	DISPAPI_ERROR_CREATEFILE		0xE0210203	// Error of create file.
#define	DISPAPI_ERROR_CREATEEVENT		0xE0210209	// Error of create event.
#define	DISPAPI_ERROR_ALREADYOPEN		0xE021020A	// Error of already opened.
#define	DISPAPI_ERROR_NOT_READY			0xE021020B	// Error of not ready
#define	DISPAPI_ERROR_BUSY				0xE021020C	// Error of busy
#define	DISPAPI_ERROR_REPORT_NAK		0xE021020D	// Error of command report.

#if !defined(TPCCTRL_H_)
typedef	enum	__OUTPUT_OPTION
{
	OUTPUT_NORMAL =0,
	OUTPUT_DIRECT,
	OUTPUT_OPTION_MAX,
}eOUTPUTOPTION;
#endif


// USBHub��ԍ\����
typedef struct {
	int		iFrontUSB_MiniB;	// FrontUSB miniB port���(Enable/Disable)
	int		iFrontUSB_A;		// FrontUSB A port���(Enable/Disable)	
} DISP_USBHUB_STATE;

#pragma pack(1)

#if !defined(TPCCTRL_H_)
// -----------------------------------
// Firmware version report struct
// -----------------------------------

// -----------------------------------
// Selfcheck report struct
// -----------------------------------
typedef union _ST_SELFCHECK_RESULT_
{
	BYTE	ucByte;
	struct
	{
		BYTE	bDisConectPanel : 1;	// bit0
		BYTE	bCalibAdjErr	: 1;	// bit1
		BYTE	bReserve		: 6;	// bit2-7
	}ucBit;
}ST_SELFCHECK_RESULT, *PST_SELFCHECK_RESULT;


#endif
#pragma pack()

/////////////////////////////////////////////////////////////////////////////////////////
//	�֐��錾
/////////////////////////////////////////////////////////////////////////////////////////
DISP_API BOOL WINAPI DispAliveCheck( int *piRetValue );
DISP_API BOOL WINAPI DispSetAliveCheckStop();
DISP_API BOOL WINAPI DispSetAliveCheckTimeout( int iTime );
DISP_API BOOL WINAPI DispGetAliveCheckTimeout( int *piTime );
DISP_API BOOL WINAPI DispGetAliveCheckInfo( int *piTime, int *piStopState );
DISP_API BOOL WINAPI DispGetAliveCheckStop( int *piStopState );
DISP_API BOOL WINAPI DispGetVersion( BYTE *pbyVersion );
DISP_API BOOL WINAPI DispGetLoaderVersion( BYTE *pbyVersion );
DISP_API BOOL WINAPI DispGetSensorValue( int *piValue, int *piError );
DISP_API BOOL WINAPI DispGetSensorRawValue( DISP_SENSOR_VALUE *pstSensor, int *piError );
DISP_API BOOL WINAPI DispReadEEPROMData( int iDevice, int iOffset, int iSize, BYTE *pbyDataBuf );
DISP_API BOOL WINAPI DispWriteEEPROMData( int iDevice, int iOffset, int iSize, BYTE *pbyDataBuf );
DISP_API BOOL WINAPI DispSelfCheck( int iDevice, BOOL *pbResult );
DISP_API BOOL WINAPI DispLoaderUpdateStart( char* pcfirmPath );
#ifdef  __cplusplus
}
#endif
