\ xcomp for a SMS with PS/2 keyboard on port A and SPI relay
\ on port B, with SD card attached
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
ARCHM XCOMP Z80A FONTC
165 LOAD  \ Sega ROM signer
Z80H XCOMPC
Z80C COREL Z80H ASMH
CREATE ~FNT CPFNT7x7
335 337 LOADR ( TMS9918 )
350 352 LOADR ( VDP )
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
: INIT VDP$ GRID$ PS2$ BLK$ (im1) ;
XWRAP
$4000 OALLOT XORG 1 ( 16K ) segasig
