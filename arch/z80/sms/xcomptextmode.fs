\ xcomp using the Text Mode if the VDP. Only works on actual
\ SMS. The Megadrive's VDP doesn't have TMS9918 modes in it.
9 CONSTS $dd00 RS_ADDR $ddca PS_ADDR $c000 HERESTART
         $bf TMS_CTLPORT $be TMS_DATAPORT
         $3f CPORT_CTL $dc CPORT_D1 $dd CPORT_D2
         1 SDC_DEVID
RS_ADDR $90 - VALUE SYSVARS
SYSVARS $409 - VALUE BLK_MEM
SYSVARS $80 + VALUE GRID_MEM
SYSVARS $83 + VALUE CPORT_MEM
SYSVARS $84 + VALUE PS2_MEM
SYSVARS $85 + VALUE SDC_MEM
ARCHM XCOMP FONTC
45 LOAD  \ Sega ROM signer
Z80A XCOMPC Z80C COREL
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
$4000 OALLOT XORG 1 ( 16K ) segasig
