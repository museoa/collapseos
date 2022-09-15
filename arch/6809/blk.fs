( ----- 000 )
6809 MASTER INDEX

301 6809 macros                302 6809 boot code
306 6809 HAL                   311 6809 assembler
320 TRS-80 Color Computer 2
( ----- 001 )
( 6809 declarations )
: 6809A ASML 311 318 LOADR 309 LOAD ( HAL flow ) ASMH ;
: 6809C 302 308 LOADR ;
: 6809H 309 310 LOADR ;
: COCO2 320 322 LOADR ;
1 CONSTANT JROPLEN -1 CONSTANT JROFF
( ----- 002 )
( 6809 Boot code. IP=Y, PS=S, RS=U  ) HERE TO ORG
FJR JRi, TO L1 ( main ) $0a ALLOT0
\ end of stable ABI
L1 FMARK ( main ) PS_ADDR # LDS, RS_ADDR # LDU,
BIN( 4 + ( BOOT ) () LDX, X+0 JMP,
LSET lblval [S+0] LDD, S+0 STD, \ to next
LSET lblcell LSET lblnext Y++ LDX, X+0 JMP,
LSET lblxt U++ STY, ( IP->RS ) PULS, Y lblnext BR JRi,
LSET lbldoes [S+0] LDX, 2 # LDD, S+0 ADDD, S+0 STD, X+0 JMP,
( ----- 003 )
CODE QUIT LSET L1 ( for ABORT ) RS_ADDR # LDU,
  BIN( $0a + ( main ) () LDX, X+0 JMP,
CODE ABORT PS_ADDR # LDS, L1 BR JRi,
CODE BYE BEGIN, BR JRi,
CODE SCNT PS_ADDR # LDD, 0 <> STS, 0 <> SUBD, PSHS, D ;CODE
CODE RCNT
  RS_ADDR # LDD, 0 <> STD, U D TFR, 0 <> SUBD, PSHS, D ;CODE
( ----- 004 )
CODE /MOD ( a b -- a/b a%b )
  16 # LDA, 0 <> STA, CLRA, CLRB, ( D=running rem ) BEGIN,
    1 # ORCC, 3 S+N ROL, ( a lsb ) 2 S+N ROL, ( a msb )
    ROLB, ROLA, S+0 SUBD,
    FJR BHSi, ( if < ) S+0 ADDD, 3 S+N DEC, ( a lsb ) THEN,
  0 <> DEC, Z? ^? BR ?JRi,
  2 S+N LDX, 2 S+N STD, ( rem ) S+0 STX, ( quotient ) ;CODE
CODE * ( a b -- a*b )
  S+0 ( bm ) LDA, 3 S+N ( al ) LDB, MUL, S+0 ( bm ) STB,
  2 S+N ( am ) LDA, 1 S+N ( bl ) LDB, MUL,
    S+0 ( bm ) ADDB, S+0 STB,
  1 S+N ( al ) LDA, 3 S+N ( bl ) LDB, MUL,
  S++ ADDA, S+0 STD, ;CODE
( ----- 005 )
LSET L1 ( X=s1 Y=s2 B=cnt ) BEGIN,
  X+ LDA, Y+ CMPA, IFNZ, RTS, THEN, DECB, Z? ^? BR ?JRi, RTS,
CODE []= ( a1 a2 u -- f TODO: allow u>$ff )
  0 <> STY, PULS, DXY ( B=u, X=a2, Y=a1 ) L1 () JSR,
  IFZ, 1 # LDD, ELSE, CLRA, CLRB, THEN, PSHS, D 0 <> LDY, ;CODE
CODE FIND ( sa sl -- w? f )
  SYSVARS $02 + ( CURRENT ) () LDX,
  0 <> STY, PULS, D 2 <> STB, BEGIN,
    -X LDB, $7f # ANDB, --X TST, 2 <> CMPB, IFZ,
      3 <> STX, S+0 LDY, NEGB, X+B LEAX, NEGB, L1 () JSR,
      IFZ, ( match ) 0 <> LDY, 3 <> LDD, 3 # ADDD, S+0 STD,
        1 # LDD, PSHS, D ;CODE THEN,
      3 <> LDX, THEN, \ nomatch, X=prev
  X+0 LDX, Z? ^? BR ?JRi, \ not zero, loop
  ( end of dict ) 0 <> LDY, S+0 STX, ( X=0 ) ;CODE
( ----- 006 )
CODE (br) LSET L1 Y+0 LDA, Y+A LEAY, ;CODE
CODE (?br) S+ LDA, S+ ORA, Z? ^? L1 BR ?JRi, Y+ TST, ;CODE
CODE (next) --U LDD, 1 # SUBD, IFNZ,
  U++ STD, L1 BR JRi, THEN, Y+ TST, ;CODE
CODE @ [S+0] LDD, S+0 STD, ;CODE
CODE C@ [S+0] LDB, CLRA, S+0 STD, ;CODE
CODE ! PULS, X PULS, D X+0 STD, ;CODE
CODE C! PULS, X PULS, D X+0 STB, ;CODE
( ----- 007 )
CODE AND PULS, D S+0 ANDA, 1 S+N ANDB, S+0 STD, ;CODE
CODE OR PULS, D S+0 ORA, 1 S+N ORB, S+0 STD, ;CODE
CODE XOR PULS, D S+0 EORA, 1 S+N EORB, S+0 STD, ;CODE
CODE + PULS, D S+0 ADDD, S+0 STD, ;CODE
CODE - 2 S+N LDD, S++ SUBD, S+0 STD, ;CODE
CODE 1+ 1 S+N INC, IFZ, S+0 INC, THEN, ;CODE
CODE 1- 1 S+N TST, IFZ, S+0 DEC, THEN, 1 S+N DEC, ;CODE
CODE << 1 S+N LSL, S+0 ROL, ;CODE
CODE >> S+0 LSR, 1 S+N ROR, ;CODE
CODE <<8 1 S+N LDA, S+0 STA, 1 S+N CLR, ;CODE
CODE >>8 S+0 LDA, 1 S+N STA, S+0 CLR, ;CODE
( ----- 008 )
CODE R@ -2 U+N LDD, PSHS, D ;CODE
CODE R~ --U TST, ;CODE
CODE R> --U LDD, PSHS, D ;CODE
CODE >R PULS, D U++ STD, ;CODE
CODE DROP 2 S+N LEAS, ;CODE
CODE DUP ( a -- a a ) S+0 LDD, PSHS, D ;CODE
CODE SWAP ( a b -- b a )
  S+0 LDD, 2 S+N LDX, S+0 STX, 2 S+N STD, ;CODE
CODE OVER ( a b -- a b a )
  2 S+N LDD, PSHS, D ;CODE
CODE ROT ( a b c -- b c a )
  4 S+N LDX, ( a ) 2 S+N LDD, ( b ) 4 S+N STD, S+0 LDD, ( c )
  2 S+N STD, S+0 STX, ;CODE
CODE ROT> ( a b c -- c a b )
  S+0 LDX, ( c ) 2 S+N LDD, ( b ) S+0 STD, 4 S+N LDD, ( a )
  2 S+N STD, 4 S+N STX, ;CODE
( ----- 009 )
\ 6809 HAL, flow words. Also used in 6809A
SYSVARS $16 + CONSTANT ?JROP
: JMPi, $7e C, M, ( jmp nn ) ; : CALLi, $bd C, M, ( jsr nn ) ;
: JRi, $20 C, C, ( bra n ) ; : ?JRi, ?JROP @ C, C, ;
: Z? $27 ?JROP ! ( beq ) ; : C? $25 ?JROP ! ( bcs ) ;
: ^? ?JROP @ 1 XOR ?JROP ! ( bne/bcc ) ;
( ----- 010 )
\ 6809 HAL
: >JMP, $3510 M, ( puls x ) $6e00 M, ( jmp 0,x ) ;
: @Z, $ae60 M, ( ldx 0,S ) ;
: i>, $cc C, M, ( ldd nn ) $3406 M, ( pshs d ) ;
: >(i), $3506 M, ( puls d ) $fd C, M, ( std (nn) ) ;
: (i)>, $fc C, M, ( ldd (nn) ) $3406 M, ( pshs d ) ;
: (i)+, $7c C, DUP 1+ M, ( inc (nn) ) $2603 M, ( bne )
  $7c C, M, ;
: (i)-, $fc C, DUP M, ( ldd (nn) ) $83 C, 1 M, ( subd )
  $fd C, M, ( std (nn) ) ;
: C>!, $cc C, 0 M, $c900 M, ( adcb 0 ) $ed60 M, ( std 0,S ) ;
: Z>!, Z? 5 ?JRi, $cc C, 0 M, 3 JRi, $cc C, 1 M,
  $ed60 M, ( std 0,S ) ;
: >IP, $3520 M, ( puls y ) ; : IP>, $3420 M, ( pshs y ) ;
: IP+, $6da0 M, ( tst ,y+ ) ;
( ----- 011 )
\ 6809 assembler. See doc/asm.txt.
1 TO BIGEND?
\ For TFR/EXG
10 CONSTS 0 D 1 X 2 Y 3 U 4 S 5 PCR 8 A 9 B 10 CCR 11 DPR
\ Addressing modes. output: n3? n2? n1 nc opoff
: # ( n ) 1 0 ; \ Immediate
: <> ( n ) 1 $10 ; \ Direct
: () ( n ) L|M 2 $30 ; \ Extended
: [] ( n ) L|M $9f 3 $20 ; \ Extended Indirect
\ Offset Indexed. We auto-detect 0, 5-bit, 8-bit, 16-bit
: _0? ?DUP IF 1 ELSE $84 1 0 THEN ;
: _5? DUP $10 + $1f > IF 1 ELSE $1f AND 1 0 THEN ;
: _8? DUP $80 + $ff > IF 1 ELSE <<8 >>8 $88 2 0 THEN ;
: _16 L|M $89 3 ;
( ----- 012 )
: R+N DOER C, DOES> C@ ( roff ) >R
    _0? IF _5? IF _8? IF _16 THEN THEN THEN
    SWAP R> ( roff ) OR SWAP $20 ;
: R+K DOER C, DOES> C@ 1 $20 ;
: PCR+N ( n ) _8? IF _16 THEN SWAP $8c OR SWAP $20 ;
: [R+N] DOER C, DOES> C@ $10 OR ( roff ) >R
    _0? IF _8? IF _16 THEN THEN SWAP R> OR SWAP $20 ;
: [PCR+N] ( n ) _8? IF _16 THEN SWAP $9c OR SWAP $20 ;
0 R+N X+N   $20 R+N Y+N  $40 R+N U+N   $60 R+N S+N
: X+0 0 X+N ; : Y+0 0 Y+N ; : S+0 0 S+N ; : U+0 0 S+N ;
0 [R+N] [X+N] $20 [R+N] [Y+N]
$40 [R+N] [U+N] $60 [R+N] [S+N]
: [X+0] 0 [X+N] ; : [Y+0] 0 [Y+N] ;
: [S+0] 0 [S+N] ; : [U+0] 0 [U+N] ;
( ----- 013 )
$86 R+K X+A   $85 R+K X+B   $8b R+K X+D
$a6 R+K Y+A   $a5 R+K Y+B   $ab R+K Y+D
$c6 R+K U+A   $c5 R+K U+B   $cb R+K U+D
$e6 R+K S+A   $e5 R+K S+B   $eb R+K S+D
$96 R+K [X+A] $95 R+K [X+B] $9b R+K [X+D]
$b6 R+K [Y+A] $b5 R+K [Y+B] $bb R+K [Y+D]
$d6 R+K [U+A] $d5 R+K [U+B] $db R+K [U+D]
$f6 R+K [S+A] $f5 R+K [S+B] $fb R+K [S+D]
$80 R+K X+  $81 R+K X++  $82 R+K -X  $83 R+K --X
$a0 R+K Y+  $a1 R+K Y++  $a2 R+K -Y  $a3 R+K --Y
$c0 R+K U+  $c1 R+K U++  $c2 R+K -U  $c3 R+K --U
$e0 R+K S+  $e1 R+K S++  $e2 R+K -S  $e3 R+K --S
$91 R+K [X++] $93 R+K [--X] $b1 R+K [Y++] $b3 R+K [--Y]
$d1 R+K [U++] $d3 R+K [--U] $f1 R+K [S++] $f3 R+K [--S]
( ----- 014 )
: ,? DUP $ff > IF M, ELSE C, THEN ;
: ,N ( cnt ) >R BEGIN C, NEXT ;
: OPINH ( inherent ) DOER , DOES> @ ,? ;
( Targets A or B )
: OP1 DOER , DOES> @ ( n2? n1 nc opoff op ) + ,? ,N ;
( Targets D/X/Y/S/U. Same as OP1, but spit 2b immediate )
: OP2 DOER , DOES> @ OVER + ,? IF ,N ELSE DROP M, THEN ;
( Targets memory only. opoff scheme is different than OP1/2 )
: OPMT DOER , DOES> @
    SWAP $10 - ?DUP IF $50 + + THEN ,? ,N ;
( Targets 2 regs )
: OPRR ( src tgt -- ) DOER C, DOES> C@ C, SWAP <<4 OR C, ;
: OPBR ( op1 -- ) DOER C, DOES> ( off -- ) C@ C, C, ;
: OPLBR ( op? -- ) DOER , DOES> ( off -- ) @ ,? M, ;
( ----- 015 )
$89 OP1 ADCA,        $c9 OP1 ADCB,
$8b OP1 ADDA,        $cb OP1 ADDB,      $c3 OP2 ADDD,
$84 OP1 ANDA,        $c4 OP1 ANDB,      $1c OP1 ANDCC,
$48 OPINH ASLA,      $58 OPINH ASLB,    $08 OPMT ASL,
$47 OPINH ASRA,      $57 OPINH ASRB,    $07 OPMT ASR,
$4f OPINH CLRA,      $5f OPINH CLRB,    $0f OPMT CLR,
$81 OP1 CMPA,        $c1 OP1 CMPB,      $1083 OP2 CMPD,
$118c OP2 CMPS,      $1183 OP2 CMPU,    $8c OP2 CMPX,
$108c OP2 CMPY,
$43 OPINH COMA,      $53 OPINH COMB,    $03 OPMT COM,
$3c OP1 CWAI,        $19 OPINH DAA,
$4a OPINH DECA,      $5a OPINH DECB,    $0a OPMT DEC,
$88 OP1 EORA,        $c8 OP1 EORB,      $1e OPRR EXG,
$4c OPINH INCA,      $5c OPINH INCB,    $0c OPMT INC,
$0e OPMT JMP,        $8d OP1 JSR,
( ----- 016 )
$86 OP1 LDA,         $c6 OP1 LDB,       $cc OP2 LDD,
$10ce OP2 LDS,       $ce OP2 LDU,       $8e OP2 LDX,
$108e OP2 LDY,
$12 OP1 LEAS,        $13 OP1 LEAU,      $10 OP1 LEAX,
$11 OP1 LEAY,
$48 OPINH LSLA,      $58 OPINH LSLB,    $08 OPMT LSL,
$44 OPINH LSRA,      $54 OPINH LSRB,    $04 OPMT LSR,
$3d OPINH MUL,
$40 OPINH NEGA,      $50 OPINH NEGB,    $00 OPMT NEG,
$12 OPINH NOP,
$8a OP1 ORA,         $ca OP1 ORB,       $1a OP1 ORCC,
$49 OPINH ROLA,      $59 OPINH ROLB,    $09 OPMT ROL,
$46 OPINH RORA,      $56 OPINH RORB,    $06 OPMT ROR,
$3b OPINH RTI,       $39 OPINH RTS,
$82 OP1 SBCA,        $c2 OP1 SBCB,
$1d OPINH SEX,
( ----- 017 )
$87 OP1 STA,         $c7 OP1 STB,       $cd OP2 STD,
$10cf OP2 STS,       $cf OP2 STU,       $8f OP2 STX,
$108f OP2 STY,
$80 OP1 SUBA,        $c0 OP1 SUBB,      $83 OP2 SUBD,
$3f OPINH SWI,       $103f OPINH SWI2,  $113f OPINH SWI3,
$13 OPINH SYNC,      $1f OPRR TFR,
$4d OPINH TSTA,      $5d OPINH TSTB,    $0d OPMT TST,

$24 OPBR BCCi,       $1024 OPLBR LBCCi, $25 OPBR BCSi,
$1025 OPLBR LBCSi,   $27 OPBR BEQi,     $1027 OPLBR LBEQi,
$2c OPBR BGEi,       $102c OPLBR LBGEi, $2e OPBR BGTi,
$102e OPLBR LBGTi,   $22 OPBR BHIi,     $1022 OPLBR LBHIi,
$24 OPBR BHSi,       $1024 OPLBR LBHSi, $2f OPBR BLEi,
$102f OPLBR LBLEi,   $25 OPBR BLOi,     $1025 OPLBR LBLOi,
$23 OPBR BLSi,       $1023 OPLBR LBLSi, $2d OPBR BLTi,
$102d OPLBR LBLTi,   $2b OPBR BMIi,     $102b OPLBR LBMIi,
( ----- 018 )
$26 OPBR BNEi,       $1026 OPLBR LBNEi, $2a OPBR BPLi,
$102a OPLBR LBPLi,   $20 OPBR BRAi,     $16 OPLBR LBRAi,
$21 OPBR BRNi,       $1021 OPLBR BRNi,  $8d OPBR BSRi,
$17 OPLBR LBSRi,     $28 OPBR BVCi,     $1028 OPLBR LBVCi,
$29 OPBR BVSi,       $1029 OPLBR LBVSi,

: _ ( r c cref mask -- r c ) ROT> OVER = ( r mask c f )
    IF ROT> OR SWAP ELSE NIP THEN ;
: OPP DOER C, DOES> C@ C, 0 TOWORD BEGIN ( r c )
    '$' $80 _ 'S' $40 _ 'U' $40 _ 'Y' $20 _ 'X' $10 _
    '%' $08 _ 'B' $04 _ 'A' $02 _ 'C' $01 _ 'D' $06 _
    '@' $ff _ DROP IN< DUP WS? UNTIL DROP C, ;
$34 OPP PSHS, $36 OPP PSHU, $35 OPP PULS, $37 OPP PULU,
( ----- 020 )
\ CoCo2 drivers
PC ," @HPX08" CR C, ," AIQY19" 0 C,
   ," BJRZ2:" 0 C,  ," CKS_3;" 0 C,
   ," DLT_4," 0 C,  ," EMU" BS C, ," 5-" 0 C,
   ," FNV_6." 0 C,  ," GOW 7/" 0 C,
   ," @hpx0(" CR C, ," aiqy!)" 0 C,
   ," bjrz" '"' C, '*' C, 0 C, ," cks_#+" 0 C,
   ," dlt_$<" 0 C,  ," emu" BS C, ," %=" 0 C,
   ," fnv_&>" 0 C,  ," gow '?" 0 C,
LSET L1 ( PC ) # LDX, $fe # LDA, BEGIN, ( 8 times )
  $ff02 () STA, ( set col ) $ff00 () LDB, ( read row )
  ( ignore 8th row ) $80 # ORB, $7f # CMPA, IFZ,
    ( ignore shift row ) $40 # ORB, THEN,
  INCB, IFNZ, ( key pressed ) DECB, RTS, THEN,
  ( inc col ) 7 X+N LEAX, 1 # ORCC, ROLA, C? BR ?JRi,
  ( no key ) CLRB, RTS,
( ----- 021 )
CODE (key?) ( -- c? f ) CLRA, CLRB, PSHS, D L1 () JSR,
  IFNZ, ( key! row mask in B col ptr in X )
    ( is shift pressed? ) $7f # LDA, $ff02 () STA,
    $ff00 () LDA, $40 # ANDA, IFZ, ( shift! )
      56 X+N LEAX, THEN,
    BEGIN, X+ LDA, LSRB, C? BR ?JRi,
    ( A = our char ) 1 S+N STA, TSTA, IFNZ, ( valid key )
      1 # LDD, ( f ) PSHS, D ( wait for keyup )
      BEGIN, L1 () JSR, Z? ^? BR ?JRi, THEN,
  THEN, ;CODE
( ----- 022 )
32 CONSTANT COLS 16 CONSTANT LINES
: CELL! ( c pos -- )
  SWAP $20 - DUP $5f < IF
    DUP $20 < IF $60 + ELSE DUP $40 < IF $20 + ELSE $40 -
      THEN THEN ( pos glyph )
    SWAP $400 + C! ELSE 2DROP THEN ;
: CURSOR! ( new old -- )
  DROP $400 + DUP C@ $40 XOR SWAP C! ;
