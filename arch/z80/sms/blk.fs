( ----- 600 )
Sega Master System Recipe

602 VDP                        610 PAD
620 KBD                        625 Ports
( ----- 602 )
( VDP Driver. requires TMS9918 driver. Load range B602-B604. )
CREATE _idat
0b00000100 C, 0x80 C, ( Bit 2: Select mode 4 )
0b00000000 C, 0x81 C,
0b00001111 C, 0x82 C, ( Name table: 0x3800, *B0 must be 1* )
0b11111111 C, 0x85 C, ( Sprite table: 0x3f00 )
0b11111111 C, 0x86 C, ( sprite use tiles from 0x2000 )
0b11111111 C, 0x87 C, ( Border uses palette 0xf )
0b00000000 C, 0x88 C, ( BG X scroll )
0b00000000 C, 0x89 C, ( BG Y scroll )
0b11111111 C, 0x8a C, ( Line counter (why have this?) )
( ----- 603 )
( Each row in ~FNT is a row of the glyph and there is 7 of
them.  We insert a blank one at the end of those 7. For each
row we set, we need to send 3 zero-bytes because each pixel in
the tile is actually 4 bits because it can select among 16
palettes. We use only 2 of them, which is why those bytes
always stay zero. )
: _sfont ( a -- Send font to VDP )
    7 0 DO C@+ _data 3 _zero LOOP DROP
    ( blank row ) 4 _zero ;
: CELL! ( c pos )
    2 * 0x7800 OR _ctl ( c )
    0x20 - ( glyph ) 0x5f MOD _data ;
( ----- 604 )
: CURSOR! ( new old -- )
    ( unset palette bit in old tile )
    2 * 1+ 0x7800 OR _ctl 0 _data
    ( set palette bit for at specified pos )
    2 * 1+ 0x7800 OR _ctl 0x8 _data ;
: VDP$
    9 0 DO _idat I 2 * + @ _ctl LOOP
    ( blank screen ) 0x7800 _ctl COLS LINES * 2 * _zero
    ( palettes )
    0xc000 _ctl
    ( BG ) 1 _zero 0x3f _data 14 _zero
    ( sprite, inverted colors ) 0x3f _data 15 _zero
    0x4000 _ctl 0x5f 0 DO ~FNT I 7 * + _sfont LOOP
    ( bit 6, enable display, bit 7, ?? ) 0x81c0 _ctl ;

: COLS 32 ; : LINES 24 ;
( ----- 610 )
Pad driver - read input from MD controller

Conveniently expose an API to read the status of a MD pad A.
Moreover, implement a mechanism to input arbitrary characters
from it. It goes as follow:

* Direction pad select characters. Up/Down move by one,
  Left/Right move by 5
* Start acts like Return
* A acts like Backspace
* B changes "character class": lowercase, uppercase, numbers,
  special chars. The space character is the first among special
  chars.
* C confirms letter selection

                                                        (cont.)
( ----- 611 )
This module is currently hard-wired to VDP driver, that is, it
calls vdp's routines during (key?) to update character
selection.

Load range: 632-637
( ----- 612 )
: _prevstat [ PAD_MEM LITN ] ;
: _sel [ PAD_MEM 1+ LITN ] ;
: _next [ PAD_MEM 2 + LITN ] ;

( Put status for port A in register A. Bits, from MSB to LSB:
Start - A - C - B - Right - Left - Down - Up
Each bit is high when button is unpressed and low if button is
pressed. When no button is pressed, 0xff is returned.
This logic below is for the Genesis controller, which is modal.
TH is an output pin that switches the meaning of TL and TR. When
TH is high (unselected), TL = Button B and TR = Button C. When
TH is low (selected), TL = Button A and TR = Start. )
( ----- 613 )
: _status
    1 _THA! ( output, high/unselected )
    _D1@ 0x3f AND ( low 6 bits are good )
( Start and A are returned when TH is selected, in bits 5 and
  4. Well get them, left-shift them and integrate them to B. )
    0 _THA! ( output, low/selected )
    _D1@ 0x30 AND 2 LSHIFT OR ;
( ----- 614 )
: _chk ( c --, check _sel range )
    _sel C@ DUP 0x7f > IF 0x20 _sel C! THEN
    0x20 < IF 0x7f _sel C! THEN ;
CREATE _ '0' C, ':' C, 'A' C, '[' C, 'a' C, 0xff C,
: _nxtcls
    _sel @ _ BEGIN ( c a ) C@+ 2 PICK > UNTIL ( c a )
    1- C@ NIP _sel !
;
( ----- 615 )
: _updsel ( -- f, has an action button been pressed? )
    _status _prevstat C@ OVER = IF DROP 0 EXIT THEN
    DUP _prevstat C! ( changed, update ) ( s )
    0x01 ( UP ) OVER AND NOT IF 1 _sel +! THEN
    0x02 ( DOWN ) OVER AND NOT IF -1 _sel +! THEN
    0x04 ( LEFT ) OVER AND NOT IF -5 _sel +! THEN
    0x08 ( RIGHT ) OVER AND NOT IF 5 _sel +! THEN
    0x10 ( BUTB ) OVER AND NOT IF _nxtcls THEN
    ( update sel in VDP )
    _chk _sel C@ XYPOS @ CELL!
    ( return whether any of the high 3 bits is low )
    0xe0 AND 0xe0 <
;
( ----- 616 )
: (key?) ( -- c? f )
    _next C@ IF _next C@ 0 _next C! 1 EXIT THEN
    _updsel IF
    _prevstat C@
    0x20 ( BUTC ) OVER AND NOT IF DROP _sel C@ 1 EXIT THEN
    0x40 ( BUTA ) AND NOT IF 0x8 ( BS ) 1 EXIT THEN
    ( If not BUTC or BUTA, it has to be START )
    0xd _next C! _sel C@ 1
    ELSE 0 ( f ) THEN ;
( ----- 617 )
: PAD$
    0xff _prevstat C! 'a' _sel C! 0 _next C! ;
( ----- 620 )
( kbd - implement (ps2kc) for SMS PS/2 adapter )
: (ps2kcA) ( for port A )
( Before reading a character, we must first verify that there
is something to read. When the adapter is finished filling its
'164 up, it resets the latch, which output's is connected to
TL. When the '164 is full, TL is low. Port A TL is bit 4 )
    _D1@ 0x10 AND IF 0 EXIT ( nothing ) THEN
    0 _THA! ( Port A TH output, low )
    _D1@ ( bit 3:0 go in 3:0 ) 0x0f AND ( n )
    1 _THA! ( Port A TH output, high )
    _D1@ ( bit 3:0 go in 7:4 ) 0x0f AND 4 LSHIFT OR ( n )
	2 _THA! ( TH input ) ;
( ----- 621 )
: (ps2kcB) ( for port B )
	( Port B TL is bit 2 )
    _D2@ 0x04 AND IF 0 EXIT ( nothing ) THEN
    0 _THB! ( Port B TH output, low )
    _D1@ ( bit 7:6 go in 1:0 ) 6 RSHIFT ( n )
    _D2@ ( bit 1:0 go in 3:2 ) 0x03 AND 2 LSHIFT OR ( n )
    1 _THB! ( Port B TH output, high )
    _D1@ ( bit 7:6 go in 5:4 ) 0xc0 AND 2 RSHIFT OR ( n )
    _D2@ ( bit 1:0 go in 7:6 ) 0x03 AND 6 LSHIFT OR ( n )
	2 _THB! ( TH input ) ;
( ----- 622 )
: (spie) DROP ; ( always enabled )
CODE (spix) ( x -- x, for port B ) HL POP, chkPS,
    ( TR = DATA TH = CLK )
    CPORT_MEM LDA(i), 0xf3 ANDi, ( TR/TH output )
    H 8 LDri, BEGIN,
        0xbf ANDi, ( TR lo ) L RL, ( --> C )
        IFC, 0x40 ORi, ( TR hi ) THEN,
        CPORT_CTL OUTiA, ( clic! ) 0x80 ORi, ( TH hi )
        CPORT_CTL OUTiA, ( clac! )
        EXAFAF', CPORT_D1 INAi, ( Up Btn is B6 ) RLA, RLA,
            E RL, EXAFAF',
        0x7f ANDi, ( TH lo ) CPORT_CTL OUTiA, ( cloc! )
    H DECr, JRNZ, AGAIN, CPORT_MEM LD(i)A,
    L E LDrr, HL PUSH,
;CODE
( ----- 625 )
( Routines for interacting with SMS controller ports.
  Requires CPORT_MEM, CPORT_CTL, CPORT_D1 and CPORT_D2 to be
  defined. CPORT_MEM is a 1 byte buffer for CPORT_CTL. The last
  3 consts will usually be 0x3f, 0xdc, 0xdd. )
( mode -- set TR pin on mode a on:
0= output low 1=output high 2=input )
CODE _TRA! HL POP, chkPS, ( B0 -> B4, B1 -> B0 )
    L RR, RLA, RLA, RLA, RLA, L RR, RLA,
    0x11 ANDi, L A LDrr, CPORT_MEM LDA(i),
    0xee ANDi, L ORr, CPORT_CTL OUTiA, CPORT_MEM LD(i)A,
;CODE
CODE _THA! HL POP, chkPS, ( B0 -> B5, B1 -> B1 )
    L RR, RLA, RLA, RLA, RLA, L RR, RLA, RLA,
    0x22 ANDi, L A LDrr, CPORT_MEM LDA(i),
    0xdd ANDi, L ORr, CPORT_CTL OUTiA, CPORT_MEM LD(i)A,
;CODE
( ----- 626 )
CODE _TRB! HL POP, chkPS, ( B0 -> B6, B1 -> B2 )
    L RR, RLA, RLA, RLA, RLA, L RR, RLA, RLA, RLA,
    0x44 ANDi, L A LDrr, CPORT_MEM LDA(i),
    0xbb ANDi, L ORr, CPORT_CTL OUTiA, CPORT_MEM LD(i)A,
;CODE
CODE _THB! HL POP, chkPS, ( B0 -> B7, B1 -> B3 )
    L RR, RLA, RLA, RLA, RLA, L RR, RLA, RLA, RLA, RLA,
    0x88 ANDi, L A LDrr, CPORT_MEM LDA(i),
    0x77 ANDi, L ORr, CPORT_CTL OUTiA, CPORT_MEM LD(i)A,
;CODE
CODE _D1@ CPORT_D1 INAi, PUSHA, ;CODE
CODE _D2@ CPORT_D2 INAi, PUSHA, ;CODE
