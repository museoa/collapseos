\ xcomp using the Text Mode if the VDP. Only works on actual
\ SMS. The Megadrive's VDP doesn't have TMS9918 modes in it.
9 CONSTS $dd00 RS_ADDR $ddca PS_ADDR $c000 HERESTART
         $bf TMS_CTLPORT $be TMS_DATAPORT
         $3f CPORT_CTL $dc CPORT_D1 $dd CPORT_D2
         1 SDC_DEVID
RS_ADDR $90 - CONSTANT SYSVARS
SYSVARS $80 + CONSTANT GRID_MEM
SYSVARS $83 + CONSTANT CPORT_MEM
SYSVARS $84 + CONSTANT PS2_MEM
SYSVARS $85 + CONSTANT SDC_MEM
120 LOAD \ nC, for PS/2 subsystem
ARCHM Z80A XCOMPL FONTC
165 LOAD  \ Sega ROM signer
Z80H XCOMPH

DI, $100 JP, $62 ALLOT0 ( $66 )
RETN, $98 ALLOT0 ( $100 )
( All set, carry on! )
$100 TO BIN(
Z80C COREL Z80H ASMH
CREATE ~FNT CPFNT5x7
335 337 LOADR ( TMS9918 )
GRIDSUB
368 369 LOADR ( SMS ports )
360 LOAD ( KBD )
: (ps2kc) (ps2kcA) ;
PS2SUB
367 LOAD ( SPI )
250 258 LOADR ( SDC )
ALIAS SDC@ (blk@)
ALIAS SDC! (blk!)
BLKSUB
: INIT TMS$ GRID$ PS2$ BLK$ (im1) ;
XWRAP
\ start/stop range for SMS is a bit special
ORG $100 - DUP TO ORG
DUP 1 ( 16K ) segasig
$4000 + HERE - ALLOT
