( ----- 600 )
PC/AT recipe

602 MBR bootloader             604 KEY/EMIT drivers
606-608 BLK drivers            610 AT-XY drivers
612 xcomp unit
( ----- 602 )
HERE ORG ! 0x7c00 BIN( ! ( BIOS loads boot bin at 0x7c00 )
JMPs, L1 FWRs ( start )
ORG @ 0x25 + H ! ( bypass BPB )
L1 FSET ( start )
CLI, CLD, AX 0x800 MOVxI, DS AX MOVsx, ES AX MOVsx,
SS AX MOVsx, DX PUSHx, ( will be popped by OS ) STI,
AH 2 MOVri, DH 0 MOVri, CH 0 MOVri, CL 2 MOVri, AL 15 MOVri,
BX 0 MOVxI, 0x13 INT, ( read sectors 2-15 of boot floppy )
( TODO: reading 12 sectors like this probably doesn't work
  on real vintage PC/AT with floppy. Make this more robust. )
0x800 0 JMPf,
ORG @ 0x1fe + H ! 0x55 C, 0xaa C,
( ----- 604 )
CODE (emit) 1 chkPS,
    AX POPx, AH 0x0e MOVri, ( print char ) 0x10 INT,
;CODE
CODE (key?)
    AH AH XORrr, 0x16 INT, AH AH XORrr, AX PUSHx, AX PUSHx,
;CODE
( ----- 606 )
CODE 13H08H ( driveno -- cx dx )
    DI POPx, DX PUSHx, ( protect ) DX DI MOVxx, AX 0x800 MOVxI,
    ES PUSHs, DI DI XORxx, ES DI MOVsx,
    0x13 INT, DI DX MOVxx, ES POPs, DX POPx, ( unprotect )
    CX PUSHx, DI PUSHx,
;CODE
CODE 13H ( ax bx cx dx -- ax bx cx dx )
    SI POPx, ( DX ) CX POPx, BX POPx, AX POPx,
    DX PUSHx, ( protect ) DX SI MOVxx, DI DI XORxx,
    0x13 INT, SI DX MOVxx, DX POPx, ( unprotect )
    AX PUSHx, BX PUSHx, CX PUSHx, SI PUSHx,
;CODE
( ----- 607 )
: FDSPT 0x70 RAM+ ;
: FDHEADS 0x71 RAM+ ;
: _ ( AX BX sec )
    ( AH=read sectors, AL=1 sector, BX=dest,
      CH=trackno CL=secno DH=head DL=drive )
    FDSPT C@ /MOD ( AX BX sec trk )
    FDHEADS C@ /MOD ( AX BX sec head trk )
    8 LSHIFT ROT OR 1+ ( AX BX head CX )
    SWAP 8 LSHIFT 0x03 C@ ( boot drive ) OR ( AX BX CX DX )
    13H 2DROP 2DROP
;
( ----- 608 )
: FD@
    2 * 16 + ( blkfs starts at sector 16 )
    0x0201 BLK( 2 PICK _
    0x0201 BLK( 0x200 + ROT 1+ _ ;
: FD!
    2 * 16 + ( blkfs starts at sector 16 )
    0x0301 BLK( 2 PICK _
    0x0301 BLK( 0x200 + ROT 1+ _ ;
: FD$
    ( get number of sectors per track with command 08H. )
    0x03 ( boot drive ) C@ 13H08H
    8 RSHIFT 1+ FDHEADS C!
    0x3f AND FDSPT C!
;
( ----- 610 )
: COLS 80 ; : LINES 25 ;
CODE AT-XY ( x y )
    ( DH=row DL=col BH=page )
    AX POPx, BX POPx, DX PUSHx, ( protect )
    DH AL MOVrr, DL BL MOVrr, BX BX XORxx, AH 2 MOVri,
    0x10 INT, DX POPx, ( unprotect )
;CODE
( ----- 612 )
0xff00 CONSTANT RS_ADDR
0xfffa CONSTANT PS_ADDR
RS_ADDR 0xa0 - CONSTANT SYSVARS
20 LOAD   ( 8086 asm )
262 LOAD  ( xcomp ) 270 LOAD  ( xcomp overrides )
442 457 LOADR ( 8086 boot code )
353 LOAD  ( xcomp core low )
604 LOAD  ( KEY/EMIT drivers )
606 608 LOADR  ( BLK drivers )
610 LOAD  ( AT-XY drivers )
390 LOAD  ( xcomp core high )
(entry) _ ( Update LATEST ) PC ORG @ 8 + !
," BLK$ FD$ ' FD@ ' BLK@* **! ' FD! ' BLK!* **! " EOT,
