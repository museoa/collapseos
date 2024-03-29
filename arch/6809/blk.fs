( ----- 000 )
6809 MASTER INDEX

301 6809 macros                302 6809 boot code
310 6809 assembler
320 TRS-80 Color Computer 2
325 6809 disassembler          340 6809 emulator
360 Virgil's workspace
( ----- 001 )
( 6809 declarations )
: 6809A 310 318 LOADR 7 LOAD ( flow ) ;
: 6809C 302 308 LOADR ;
: 6809D 325 336 LOADR ; : 6809E 340 354 LOADR ;
: COCO2 320 LOAD 322 324 LOADR ;
: DGN32 321 LOAD 322 324 LOADR ;
( ----- 002 )
\ 6809 Boot code. IP=Y, PS=S, RS=U
PS_ADDR # LDS, RS_ADDR # LDU, 0 () JMP, PC 2 - TO lblboot
LSET lblval SYSVARS $18 ( TO? ) + () TST, FJR BNE, TO L1
  ( val rd ) [S+0] LDD, S+0 STD, \ to next
LSET lblcell LSET lblnext Y++ LDX, X+0 JMP,
L1 FMARK ( val wr ) SYSVARS $18 + () CLR, 2 S+N LDD,
  [S++] STD, S++ TST, lblnext BR BRA,
LSET lblxt U++ STY, ( IP->RS ) PULS, Y lblnext BR JRi,
LSET lbldoes [S+0] LDX, 2 # LDD, S+0 ADDD, S+0 STD, X+0 JMP,
CODE QUIT LSET L1 ( for ABORT )
  RS_ADDR # LDU, 0 () JMP, PC 2 - TO lblmain
CODE ABORT PS_ADDR # LDS, L1 BR JRi,
CODE BYE BEGIN, BR JRi,
CODE EXIT --U LDY, ;CODE
CODE EXECUTE PULS, X X+0 JMP,
( ----- 003 )
CODE SCNT PS_ADDR # LDD, 0 <> STS, 0 <> SUBD, PSHS, D ;CODE
CODE RCNT
  RS_ADDR # LDD, 0 <> STD, U D TFR, 0 <> SUBD, PSHS, D ;CODE
CODE @ [S+0] LDD, S+0 STD, ;CODE
CODE C@ [S+0] LDB, CLRA, S+0 STD, ;CODE
CODE ! PULS, X PULS, D X+0 STD, ;CODE
CODE C! PULS, X PULS, D X+0 STB, ;CODE
LSET L1 ( PUSH Z ) CCR B TFR, LSRB, LSRB,
  1 # ANDB, CLRA, S+0 STD, ;CODE
CODE = PULS, D S+0 CMPD, L1 BR BRA, ( PUSH Z )
CODE NOT S+0 LDB, 1 S+N ORB, L1 BR BRA, ( PUSH Z )
CODE <
  2 S+N LDD, S++ CMPD, CCR B TFR, 1 # ANDB, CLRA, S+0 STD, ;CODE
( ----- 004 )
CODE /MOD ( a b -- a/b a%b )
  16 # LDA, 0 <> STA, CLRA, CLRB, ( D=running rem ) BEGIN,
    1 # ORCC, 3 S+N ROL, ( a lsb ) 2 S+N ROL, ( a msb )
    ROLB, ROLA, S+0 SUBD,
    FJR BHS, ( if < ) S+0 ADDD, 3 S+N DEC, ( a lsb ) THEN,
  0 <> DEC, BR JRNZi,
  2 S+N LDX, 2 S+N STD, ( rem ) S+0 STX, ( quotient ) ;CODE
CODE * ( a b -- a*b )
  S+0 ( bm ) LDA, 3 S+N ( al ) LDB, MUL, S+0 ( bm ) STB,
  2 S+N ( am ) LDA, 1 S+N ( bl ) LDB, MUL,
    S+0 ( bm ) ADDB, S+0 STB,
  1 S+N ( al ) LDA, 3 S+N ( bl ) LDB, MUL,
  S++ ADDA, S+0 STD, ;CODE
( ----- 005 )
LSET L1 ( X=s1 Y=s2 B=cnt ) BEGIN,
  X+ LDA, Y+ CMPA, IFNZ, RTS, THEN, DECB, BR JRNZi, RTS,
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
  X+0 LDX, BR JRNZi, \ not zero, loop
  ( end of dict ) 0 <> LDY, S+0 STX, ( X=0 ) ;CODE
( ----- 006 )
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
( ----- 007 )
CODE R@ -2 U+N LDD, PSHS, D ;CODE
CODE R~ --U TST, ;CODE
CODE R> --U LDD, PSHS, D ;CODE
CODE >R PULS, D U++ STD, ;CODE
CODE DROP 2 S+N LEAS, ;CODE
CODE DUP ( a -- a a ) S+0 LDD, PSHS, D ;CODE
CODE ?DUP ( a -- a? a ) S+0 LDD, IFNZ, PSHS, D THEN, ;CODE
CODE SWAP ( a b -- b a )
  S+0 LDD, 2 S+N LDX, S+0 STX, 2 S+N STD, ;CODE
CODE OVER ( a b -- a b a ) 2 S+N LDD, PSHS, D ;CODE
CODE ROT ( a b c -- b c a )
  4 S+N LDX, ( a ) 2 S+N LDD, ( b ) 4 S+N STD, S+0 LDD, ( c )
  2 S+N STD, S+0 STX, ;CODE
CODE ROT> ( a b c -- c a b )
  S+0 LDX, ( c ) 2 S+N LDD, ( b ) S+0 STD, 4 S+N LDD, ( a )
  2 S+N STD, 4 S+N STX, ;CODE
( ----- 008 )
CODE (b) Y+ LDB, CLRA, PSHS, D ;CODE
CODE (n) Y++ LDD, PSHS, D ;CODE
CODE (br) LSET L1 Y+0 LDA, Y+A LEAY, ;CODE
CODE (?br) S+ LDA, S+ ORA, L1 BR JRZi, Y+ TST, ;CODE
CODE (next) --U LDD, 1 # SUBD, IFNZ,
  U++ STD, L1 BR JRi, THEN, Y+ TST, ;CODE
( ----- 010 )
\ 6809 assembler. See doc/asm.txt.
'? BIGEND? [IF] 1 TO BIGEND? [THEN]
: <<3 << << << ; : <<4 <<3 << ;
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
( ----- 011 )
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
( ----- 012 )
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
( ----- 013 )
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
( ----- 014 )
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
( ----- 015 )
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
( ----- 016 )
$87 OP1 STA,         $c7 OP1 STB,       $cd OP2 STD,
$10cf OP2 STS,       $cf OP2 STU,       $8f OP2 STX,
$108f OP2 STY,
$80 OP1 SUBA,        $c0 OP1 SUBB,      $83 OP2 SUBD,
$3f OPINH SWI,       $103f OPINH SWI2,  $113f OPINH SWI3,
$13 OPINH SYNC,      $1f OPRR TFR,
$4d OPINH TSTA,      $5d OPINH TSTB,    $0d OPMT TST,

$24 OPBR BCC,        $1024 OPLBR LBCC,  $25 OPBR BCS,
$1025 OPLBR LBCS,    $27 OPBR BEQ,      $1027 OPLBR LBEQ,
$2c OPBR BGE,        $102c OPLBR LBGE,  $2e OPBR BGT,
$102e OPLBR LBGT,    $22 OPBR BHI,      $1022 OPLBR LBHI,
$24 OPBR BHS,        $1024 OPLBR LBHS,  $2f OPBR BLE,
$102f OPLBR LBLE,    $25 OPBR BLO,      $1025 OPLBR LBLO,
$23 OPBR BLS,        $1023 OPLBR LBLS,  $2d OPBR BLT,
$102d OPLBR LBLT,    $2b OPBR BMI,      $102b OPLBR LBMI,
( ----- 017 )
$26 OPBR BNE,        $1026 OPLBR LBNE,  $2a OPBR BPL,
$102a OPLBR LBPL,    $20 OPBR BRA,      $16 OPLBR LBRA,
$21 OPBR BRN,        $1021 OPLBR LBRN,  $8d OPBR BSR,
$17 OPLBR LBSR,      $28 OPBR BVC,      $1028 OPLBR LBVC,
$29 OPBR BVS,        $1029 OPLBR LBVS,

: _ ( r c cref mask -- r c ) ROT> OVER = ( r mask c f )
    IF ROT> OR SWAP ELSE NIP THEN ;
: OPP DOER C, DOES> C@ C, 0 TOWORD IN> 1- C@ BEGIN ( r c )
    '$' $80 _ 'S' $40 _ 'U' $40 _ 'Y' $20 _ 'X' $10 _
    '%' $08 _ 'B' $04 _ 'A' $02 _ 'C' $01 _ 'D' $06 _
    '@' $ff _ DROP IN< DUP WS? UNTIL DROP C, ;
$34 OPP PSHS, $36 OPP PSHU, $35 OPP PULS, $37 OPP PULU,
( ----- 018 )
\ 6809 HAL, flow words. Also used in 6809A
: JMPi, () JMP, ; : CALLi, () JSR, ; : JMP(i), [] JMP, ;
ALIAS BRA, JRi,
ALIAS BEQ, JRZi, ALIAS BNE, JRNZi,
ALIAS BCS, JRCi, ALIAS BCC, JRNCi,
: i>, # LDD, $3406 M, ( pshs d ) ;
: (i)>, () LDD, $3406 M, ( pshs d ) ;
( ----- 020 )
\ CoCo2 keyboard layout
PC ," @HPX08" CR C, ," AIQY19" 0 C,
   ," BJRZ2:" 0 C,  ," CKS_3;" 0 C,
   ," DLT_4," 0 C,  ," EMU" BS C, ," 5-" 0 C,
   ," FNV_6." 0 C,  ," GOW 7/" 0 C,
   ," @hpx0(" CR C, ," aiqy!)" 0 C,
   ," bjrz" '"' C, '*' C, 0 C, ," cks_#+" 0 C,
   ," dlt_$<" 0 C,  ," emu" BS C, ," %=" 0 C,
   ," fnv_&>" 0 C,  ," gow '?" 0 C,
( ----- 021 )
\ Dragon32 keyboard layout
PC ," 08@HPX" CR C, ," 19AIQY" 0 C,
   ," 2:BJRZ" 0 C,  ," 3;CKS_" 0 C,
   ," 4,DLT_" 0 C,  ," 5-EMU" BS C, 0 C,
   ," 6.FNV_" 0 C,  ," 7/GOW " 0 C,
   ," 0(@hpx" CR C, ," !)aiqy" 0 C,
   '"' C, '*' C, ," bjrz" 0 C, ," #+cks_" 0 C,
   ," $<dlt_" 0 C,  ," %=emu" BS C, 0 C,
   ," &>fnv_" 0 C,  ," '?gow " 0 C,
( ----- 022 )
\ Coco2 keyboard driver
LSET L1 ( PC ) # LDX, $fe # LDA, BEGIN, ( 8 times )
  $ff02 () STA, ( set col ) $ff00 () LDB, ( read row )
  ( ignore 8th row ) $80 # ORB, $7f # CMPA, IFZ,
    ( ignore shift row ) $40 # ORB, THEN,
  INCB, IFNZ, ( key pressed ) DECB, RTS, THEN,
  ( inc col ) 7 X+N LEAX, 1 # ORCC, ROLA, BR JRCi,
  ( no key ) CLRB, RTS,
( ----- 023 )
\ Coco2 keyboard driver
CODE (key?) ( -- c? f ) CLRA, CLRB, PSHS, D L1 () JSR,
  IFNZ, ( key! row mask in B col ptr in X )
    ( is shift pressed? ) $7f # LDA, $ff02 () STA,
    $ff00 () LDA, $40 # ANDA, IFZ, ( shift! )
      56 X+N LEAX, THEN,
    BEGIN, X+ LDA, LSRB, BR JRCi,
    ( A = our char ) 1 S+N STA, TSTA, IFNZ, ( valid key )
      1 # LDD, ( f ) PSHS, D ( wait for keyup )
      BEGIN, L1 () JSR, BR JRNZi, THEN,
  THEN, ;CODE
( ----- 024 )
\ Coco2 grid driver
32 CONSTANT COLS 16 CONSTANT LINES
: CELL! ( c pos -- )
  SWAP $20 - DUP $5f < IF
    DUP $20 < IF $60 + ELSE DUP $40 < IF $20 + ELSE $40 -
      THEN THEN ( pos glyph )
    SWAP $400 + C! ELSE 2DROP THEN ;
: CURSOR! ( new old -- )
  DROP $400 + DUP C@ $40 XOR SWAP C! ;
( ----- 025 )
\ 6809 disassembler
\ order below represent opid, alpha order, branches last
CREATE OPNAME ," ABXADCADDANDASLASRBITCLRCMPCOMCWADAA" \ x12
  ," DECEOREXGINCJMPJSR LDLEALSRMULNEGNOP ORPSHPULROL" \ x16
  ," RORRTIRTSSBCSEX STSUBSWISYNTFRTSTBSR" \ x12
  ," BRABRNBHIBLSBCCBCSBNEBEQBVCBVSBPLBMIBGEBLTBGTBLE" \ x16
  ," ???"
3 CONSTS 56 OPCNT $ff NUL 40 BRIDX
10 VALUES addr oplen page opcode opid modeid arg indid indR
  tgtid
: >>4 >> >> >> >> ; : n, ( n -- ) >R BEGIN RUN1 , NEXT ;
: signext ( b -- n ) DUP $7f > IF $ff00 OR THEN ;
: M@ ( a -- n ) C@+ <<8 SWAP C@ OR ; : M@+ DUP 1+ 1+ SWAP M@ ;
: WORDTBL ( n -- ) CREATE >R BEGIN ' , NEXT ;
: opname. ( -- ) opid BRIDX >= page 1 = AND IF ( Lbranch )
  'L' EMIT THEN opid OPCNT MIN 3 * OPNAME + 3 STYPE ;
( ----- 026 )
\ NEG, COM, LSR, ...
CREATE GRP0 $10 nC, 22 NUL NUL 9   20 NUL 28 5
                     4 27  12  NUL 15 38  16 7
\ NOP, SYNC, DAA, ...
CREATE GRP1 $10 nC, NUL NUL 23  36 NUL NUL NUL NUL
                    NUL  11 24 NUL   3  32  14  37
\ branches
CREATE GRP2 $10 nC, 40 41 42 43 44 45 46 47
                    48 49 50 51 52 53 54 55
\ LEA, PSH, PUL, ...
CREATE GRP3 $10 nC, 19  19 19 19 25 26 25  26
                    NUL 30  0 29 10 21 NUL 35
\ SUB, CMP, SBC, ...
CREATE GRP8 $10 nC, 34 8 31 34  3  6 18 NUL
                    13 1 24  2  8 39 18 NUL
( ----- 027 )
\ GRP8 + ST + JSR
CREATE GRP9 $10 nC, 34 8 31 34  3  6 18 33
                    13 1 24  2  8 17 18 33
\ SUB, CMP, ADD, ...
CREATE GRPC $10 nC, 34 8 31  2  3   6 18 NUL
                    13 1 24  2 18 NUL 18 NUL
\ GRPC + ST
CREATE GRPD $10 nC, 34 8 31  2  3   6 18 33
                    13 1 24  2 18  33 18 33
CREATE _ $10 n, GRP0 GRP1 GRP2 GRP3 GRP0 GRP0 GRP0 GRP0
                GRP8 GRP9 GRP9 GRP9 GRPC GRPD GRPD GRPD
: opid@ opcode DUP >>4 << _ + @ SWAP $f AND + C@ TO opid ;
( ----- 028 )
\ tgt id is the same as in TFR/EXG. 2b for each name. $6 is for
\ memory or inherent targets. $c and $d are for SWI
CREATE TGTNAME ," D X Y U S PC  ??A B CCDP2 3 ??"
CREATE GRP0 $10 nC, 6 6 6 6 6 6 6 6 6 6 6 6 6 6 6 6
CREATE GRP1 $10 nC, 6 6 6 6 6 6 6 6 6 6 10 6 10 6 6 6
CREATE GRP3 $10 nC, 1 2 4 3 4 4 3 3 6 6 6 6 6 6 6 6
CREATE GRP4 $10 nC, 8 8 8 8 8 8 8 8 8 8 8 8 8 8 8 8
CREATE GRP5 $10 nC, 9 9 9 9 9 9 9 9 9 9 9 9 9 9 9 9
CREATE GRP8 $10 nC, 8 8 8 0 8 8 8 8 8 8 8 8 1 6 1 1
CREATE GRPC $10 nC, 9 9 9 0 9 9 9 9 9 9 9 9 0 0 3 3
CREATE _ $10 n, GRP0 GRP1 GRP0 GRP3 GRP4 GRP5 GRP0 GRP0
                GRP8 GRP8 GRP8 GRP8 GRPC GRPC GRPC GRPC
: tgtid@ opcode DUP >>4 << _ + @ SWAP $f AND + C@ TO tgtid ;
: tgtwide? ( tgtid -- f ) 6 < ;
: tgtname ( tgtid -- 's ) $e MIN << TGTNAME + ;
( ----- 029 )
\ 6809D, index modes
\ 0=n,R 1=R,R 2=,R+ 3=,R++ 4=,-R 5=,--R 6=n,PC
\ 8=[n,R] 9=[R,R] 11=[,R++] 13=[,--R] 14=[n,PC] 15=iext
CREATE _ 8 nC, $02 $03 $04 $05 $00 $81 $91 $ff
: _1RRi0xxx ( n&7 -- off indid ) _ + C@ DUP >>4 SWAP $f AND ;
: _nul ( a -- a+n off indid ) 0 $ff ;
: _b,R C@+ 0 ; : _n,R M@+ 0 ;
: _D,R 0 1 ;
: _b,PC C@+ 6 ; : _n,PC M@+ 6 ;
8 WORDTBL _ _b,R _n,R _nul _D,R _b,PC _n,PC _nul _nul
: _1RRi1xxx ( a n&f -- a+n off indid )
  7 AND << _ + @ EXECUTE ;
( ----- 030 )
\ 6809D, index modes
: _R@ ( n -- R ) $60 AND >>4 >> 1+ ;
0 VALUE _i
\ R is a tgtid, off in R,R is a tgtid, an abs EA in iext.
: ind@ ( a -- a+n R off indid )
  C@+ DUP TO _i ( a n ) $9e AND $9c = IF ( iext )
    _i 1 AND IF M@+ ELSE C@+ THEN 0 SWAP 15 EXIT THEN ( a )
  _i $80 AND NOT IF ( 5b,R ) _i _R@ _i $1f AND 0 EXIT THEN ( a )
  _i $f AND DUP 7 > IF _1RRi1xxx ELSE _1RRi0xxx THEN
  _i $10 AND >> OR \ apply indirect bit to indid
  ( a off indid ) _i _R@ ROT> ;
( ----- 031 )
\ 6809D, addr modes
\ 0=inh 1=acc 2=imm 3=rel 4=zp 5=regTFR 6=regPSH 7=ext 8=ind
: nop ( a -- a+n arg ) 0 ;
: imm tgtid tgtwide? IF M@+ ELSE C@+ THEN ;
: rel page 1 = IF ( lbranch ) M@+ ELSE C@+ signext THEN ;
: zp C@+ ;
: ext M@+ ;
: ind ind@ TO indid SWAP TO indR ( a+n arg ) ;
9 WORDTBL _ nop nop imm rel zp zp zp ext ind
: arg@+ ( a -- a+n ) modeid << _ + @ EXECUTE TO arg ;
( ----- 032 )
\ 6809D, addr modes
\ 0=inh 1=acc 2=imm 3=rel 4=zp 5=regTFR 6=regPSH 7=ext 8=ind
\ $10 regular modes followed by 1x and 3x special modes
CREATE modes $30 nC, 4 $10 3 $20 1 1 8 7 2 4 8 7 2 4 8 7
  0 0 0 0 0 0 3 3 0 0 2 0 2 0 5 5
  8 8 8 8 6 6 6 6 0 0 0 0 2 0 0 0
: mode@+ ( a -- a+n )
  opcode >>4 modes + C@ DUP $10 >= IF ( offset )
    modes + opcode $f AND + C@ THEN TO modeid
  opcode $8d = IF ( BSR is special ) 3 TO modeid THEN arg@+ ;
( ----- 033 )
CREATE s 9 ALLOT
: s$ s 9 SPC FILL ;
: nul. s 9 '?' FILL ; ALIAS NOOP inh. ALIAS NOOP acc.
: imm. '#' s C! arg s 1+
  tgtid tgtwide? IF FMTX ELSE FMTx THEN 2DROP ;
: zp. '$' s C! arg s 1+ FMTx 2DROP ;
: ext. '$' s C! arg s 1+ FMTX 2DROP ;
: rel. page 1 = IF ext. ELSE zp. THEN ;
: regTFR. arg >>4 tgtname s 2 MOVE
  arg $f AND tgtname s 1+ 1+ 2 MOVE ;
CREATE _ ," $SYX%BAC"
: regPSH. A>R s >A _ s 8 MOVE arg $80 BEGIN
  2DUP AND NOT IF SPC AC! THEN A+ >> DUP NOT UNTIL 2DROP R>A ;
( ----- 034 )
\ 6809D, index modes printing
: fmt ( n -- ) DUP $ff > IF A> FMTX ELSE A> FMTx THEN 2DROP ;
: Rfmt ( tgtid -- ) tgtname C@ AC! A+ ;
: +> '+' AC! A+ ; : -> '-' AC! A+ ;
: _n,R indR Rfmt +> arg fmt ;
: _R,R indR Rfmt +> arg Rfmt ;
: _,R+ indR Rfmt +> ; : _,R++ _,R+ +> ;
: _,-R -> indR Rfmt ; : _,--R -> _,-R ;
: _n,PC S" PC+" A> SWAP MOVE A+ A+ A+ arg fmt ;
8 WORDTBL _ _n,R _R,R _,R+ _,R++ _,-R _,--R _n,PC nul.
: ind. A>R s >A indid 7 > IF '[' AC! A+ ']' A> 7 + C! THEN
  indid 7 AND << _ + @ EXECUTE R>A ;
( ----- 035 )
$10 WORDTBL _ inh. acc. imm. rel. zp. regTFR. regPSH. ext.
  ind. nul. nul. nul. nul. nul. nul. nul.
: mode. s$ modeid << _ + @ EXECUTE s 9 STYPE ;
: repl ( val tbl -- nval-or-ff *A* ) >A BEGIN
  AC@+ 2DUP = IF 2DROP AC@+ EXIT THEN A+ $ff = UNTIL DROP $ff ;
CREATE _ops 11 nC, 35 35 34 8 8 8 18 18 33 33 $ff
CREATE _tgt 9 nC, 6 12 0 0 1 2 3 4 $ff
: _op10 opid DUP BRIDX < IF _ops repl TO opid THEN
  tgtid _tgt repl TO tgtid ;
( ----- 036 )
CREATE _ops 7 nC, 35 35 34 8 8 8 $ff
CREATE _tgt 7 nC, 6 13 0 3 1 4 $ff
: _op11 opid _ops repl TO opid tgtid _tgt repl TO tgtid ;
: bin. oplen >R addr BEGIN C@+ .x SPC> NEXT DROP ;
: op@+ ( a -- a+n ) 0 TO page DUP TO addr C@+
  DUP $fe AND $10 = IF 1 AND 1+ TO page C@+ THEN ( a+n opc )
  TO opcode opid@ tgtid@ mode@+ ( a+n ) DUP addr - TO oplen
  page 1 = IF _op10 THEN page 2 = IF _op11 THEN ( a+n ) ;
: op. opname. tgtid tgtname 2 STYPE mode. SPC> bin. NL> ;
20 VALUE DISCNT
: dis ( a -- ) DISCNT >R BEGIN op@+ op. NEXT DROP ;
( ----- 040 )
\ mapping: D X Y U S PC CC/DP
CREATE 'D 14 ALLOT
'D VALUE 'A 'A 1+ VALUE 'B 'D 2 + VALUE 'X
'X 2 + VALUE 'Y 'Y 2 + VALUE 'U 'U 2 + VALUE 'S
'S 2 + VALUE 'PC 'PC 2 + VALUE 'CC 'CC 1+ VALUE 'DP
CREATE MEM $800 ALLOT \ 2K ought to be enough for anybody
\ EA is in *target* addr
4 VALUES EA HALT? VERBOSE 'BRK?
: BRK? 'BRK? DUP IF EXECUTE THEN ;
: M! ( n a -- ) OVER >>8 OVER C! 1+ C! ;
: MEM+ ( off -- addr ) MEM + ;
( ----- 041 )
\ tgtid is from 6809D
CREATE _ $10 n, 'D 'X 'Y 'U 'S 'PC 0 0 'A 'B 'CC 'DP 0 0 0 0
: tgtreg ( tgtid -- regaddr )
  $f AND << _ + @ DUP NOT IF ABORT" invalid tgt" THEN ;
: 'tgt ( tgtid -- a ) \ host addr for tgtid
  DUP 6 = IF DROP EA MEM+ ELSE tgtreg THEN ;
: CC@ 'CC C@ ; : CC! 'CC C! ;
: neg? ( n -- f ) tgtid tgtwide? NOT IF <<8 THEN 0< ;
: ZNV! ( old new -- ) <<8 >>8
  ( Z? ) DUP NOT ( old new z ) ( N? ) SWAP neg? ( old z n )
  ( V? n != oldn ) ROT neg? OVER = NOT ( z n v )
  << SWAP <<3 OR ( z f ) SWAP << << OR ( f=0000NZV0 )
  CC@ $f1 AND OR CC! ;
( ----- 042 )
: W?@ tgtid tgtwide? IF M@ ELSE C@ THEN ;
: W?! tgtid tgtwide? IF M! ELSE C! THEN ;
: PC@ 'PC M@ ; : PC! 'PC M! ;
: EA@@ EA MEM+ W?@ ; : EA@! EA MEM+ W?! ;
: tgt@ ( tgtid -- n )
  DUP 'tgt SWAP tgtwide? IF M@ ELSE C@ THEN ;
: tgt! ( n tgtid -- )
  DUP 'tgt SWAP tgtwide? IF M! ELSE C! THEN ;
( ----- 043 )
: push8 'S M@ 1- DUP 'S M! MEM+ C! ;
: push16 DUP push8 >>8 push8 ;
: pull8 'S M@ DUP 1+ 'S M! MEM+ C@ ;
: pull16 pull8 pull8 <<8 OR ;
: carry> ( -- f ) CC@ $01 AND ;
: >carry ( f -- ) CC@ $fe AND OR CC! ;
: >>CC ( b -- b>>1 ) DUP 1 AND >carry >> ;
: <<CC ( b -- b<<1 ) << DUP $ff > >carry ;
: cpu. ." A B  X    Y    U    S    PC   CC DP" NL>
  'D 6 >R BEGIN DUP M@ .X SPC> 1+ 1+ NEXT DROP
  CC@ .x SPC> 'DP C@ .x NL> ;
: NIL ." invalid opcode " NL> cpu. ABORT ;
( ----- 044 )
\ all same signature
: n,R ( -- ea ) indR tgt@ arg + ;
: R,R indR tgt@ arg tgt@ + ;
: ,R+ indR tgt@ DUP 1+ indR tgt! ;
: ,R++ indR tgt@ DUP 1+ 1+ indR tgt! ;
: ,-R indR tgt@ 1- DUP indR tgt! ;
: ,--R indR tgt@ 1- 1- DUP indR tgt! ;
: n,PC 'PC M@ arg + ;
: indirect DOER ' , DOES> @ EXECUTE MEM+ M@ ;
indirect [n,R] n,R              indirect [R,R] R,R
indirect [,R++] ,R++            indirect [,--R] ,--R
indirect [n,PC] n,PC            indirect [n] arg
( ----- 045 )
\ maps indid from 6809D
16 WORDTBL IMODES
  n,R   R,R   ,R+ ,R++   ,-R ,--R   n,PC   NIL
  [n,R] [R,R] NIL [,R++] NIL [,--R] [n,PC] [n]
: indexed indid << IMODES + @ EXECUTE ( ea ) TO EA ;
( ----- 046 )
: imm addr MEM - oplen + 1- tgtid tgtwide? IF 1- THEN TO EA ;
: rel PC@ oplen + arg + TO EA ;
: zp arg 'DP C@ <<8 + TO EA ;
: ext arg TO EA ;
9 WORDTBL ADDRS NOOP NOOP imm rel zp NOOP NOOP ext indexed
: setEA ( -- ) modeid << ADDRS + @ EXECUTE ;
: TGT!ZNV ( n -- ) \ write n to TGT and update flags
  tgtid tgt@ OVER ( n old n ) ZNV! ( n ) tgtid tgt! ;
( ----- 047 )
\ ops all have a neutral sig and expect EA and tgt to be set.
\ INH and special words
: TODO ABORT" TODO" ; : RTS TODO ; : ABX TODO ; : RTI TODO ;
: CWAI TODO ; : SWI TODO ;
ALIAS NOOP NOP
: CLR 0 tgtid tgt! CC@ $f0 AND $04 OR CC! ;
: JMP EA PC! ;
: JSR PC@ push16 EA PC! ; ALIAS JSR BSR
: MUL 'A C@ 'B C@ * DUP 'D M! ( n )
  DUP NOT << << ( n z ) SWAP $80 AND << >>8 ( z c ) OR
  ( f = 00000Z0C ) CC@ $fa AND OR CC! ;
( ----- 048 )
: SYNC 1 TO HALT? ;
: DAA TODO ;
: SEX 'B C@ signext DUP 'D M! DUP ZNV! ;
: src arg >>4 ; : dst arg $f AND ;
: EXG src tgt@ dst tgt@ >R dst tgt! R> dst tgt! ;
: TFR src tgt@ dst tgt! ;
( ----- 049 )
: CBRA 0 AND ; : CBHI 5 AND ; : CBCC 1 AND ; : CBNE 4 AND ;
: CBVC 2 AND ; : CBPL 8 AND ;
: CBGE DUP CBVC SWAP CBPL = NOT ;
: CBGT DUP CBGE SWAP CBNE OR ;
8 WORDTBL BRCOND CBRA CBHI CBCC CBNE CBVC CBPL CBGE CBGT
: BROP ( -- ) CC@ opcode >> 7 AND << BRCOND + @ EXECUTE ( f )
  opcode 1 AND XOR NOT IF JMP THEN ;
( ----- 050 )
\ TGTOP: Read TGT value, operate, then write to TGT and flags
: TGTOP ' DOER , DOES> @ tgtid tgt@ SWAP EXECUTE TGT!ZNV ;
: asr ( n -- n ) DUP >>CC SWAP $80 AND OR ; TGTOP asr ASR
: com $ff XOR ; TGTOP com COM
TGTOP 1- DEC TGTOP 1+ INC
TGTOP <<CC ASL TGTOP >>CC LSR
: neg 0 -^ ; TGTOP neg NEG
: rol carry> SWAP <<CC OR ; TGTOP rol ROL
: ror carry> <<8 >> OR >>CC ; TGTOP ror ROR
: lea DROP EA ; TGTOP lea LEA
( ----- 051 )
\ EAOP: Read TGT and EA, apply op, write to TGT and flags
: EAOP ' DOER , DOES>
  @ tgtid tgt@ EA@@ ROT EXECUTE TGT!ZNV ;
: +c ( a b -- a+b carry ) <> ( l h ) OVER + ( h sum ) TUCK > ;
: adc ( a b -- n ) tgtid tgtwide? IF +c SWAP carry> +c ROT OR
  ELSE + carry> + DUP >>8 THEN >carry ;
EAOP adc ADC
: add 0 >carry adc ; EAOP add ADD
: -c ( a b -- a-b carry ) 2DUP < ROT> - SWAP ;
: sbc -c SWAP carry> -c ROT OR >carry ; EAOP sbc SBC
: sub 0 >carry sbc ; EAOP sub SUB
EAOP AND AND_ EAOP XOR EOR EAOP OR OR_ EAOP NIP LD
( ----- 052 )
\ FLAGOP: Reads TGT, perform op, then update NVZ flags only
\ op sig: n -- n
: FLAGOP ' DOER , DOES>
  @ tgtid tgt@ DUP ROT EXECUTE ( old new ) ZNV! ;
FLAGOP NOOP TST
: cmp EA@@ sub ; FLAGOP cmp CMP
: st DUP EA@! ; FLAGOP st ST
: bit EA@@ AND ; FLAGOP bit BIT
( ----- 053 )
CREATE _ 8 nC, 10 8 9 11 1 2 4 5
: PSHS _ 7 + arg BEGIN ( a flags )
    DUP $80 AND IF OVER C@ DUP tgtreg ( a flags tgtid reg )
      SWAP tgtwide? IF M@ push16 ELSE C@ push8 THEN THEN
    << $ff AND SWAP 1- SWAP ?DUP NOT UNTIL DROP ;
: PULS _ arg BEGIN ( a flags )
    DUP 1 AND IF OVER C@ DUP tgtreg ( a flags tgtid reg )
      SWAP tgtwide? IF pull16 M! ELSE pull8 C! THEN THEN
    >> SWAP 1+ SWAP ?DUP NOT UNTIL DROP ;
: U<>S 'U @ 'S @ SWAP 'S ! 'U ! ;
: PSHU U<>S PSHS U<>S ; : PULU U<>S PULS U<>S ;
: PSH tgtid 3 = IF PSHU ELSE PSHS THEN ;
: PUL tgtid 3 = IF PULU ELSE PULS THEN ;
( ----- 054 )
BRIDX WORDTBL OPS ABX ADC ADD AND_ ASL ASR BIT CLR CMP COM CWAI
  DAA DEC EOR EXG INC JMP JSR LD LEA LSR MUL NEG NOP OR PSH PUL
  ROL ROR RTI RTS SBC SEX ST SUB SWI SYNC TFR TST BSR
: rdop PC@ MEM+ op@+ DROP ; : peekop rdop op. ;
: run1
  HALT? IF ABORT" CPU halted" THEN
  rdop VERBOSE IF op. THEN setEA PC@ oplen + PC! opid
  DUP OPCNT < IF DUP BRIDX < IF << OPS + @ EXECUTE
    ELSE DROP BROP THEN ELSE NIL THEN
  VERBOSE IF cpu. THEN
  BRK? IF ABORT" breakpoint reached" THEN ;
: runN >R BEGIN run1 NEXT ; : run BEGIN run1 AGAIN ;
: 6809E$ 0 TO HALT? $100 PC! 0 'DP C! ;
( ----- 060 )
\ test a few ops in 6809E
6809E$ 1 TO VERBOSE
HERE MEM $100 + 'HERE !
$42 # LDA, $56 # ADDA, $12 <> STA, $12 <> SUBB, SYNC,
'HERE !
( ----- 061 )
\ test new lblval semantics
$200 VALUE TO?
6809E$ 1 TO VERBOSE
HERE MEM $100 + 'HERE ! $100 XSTART
FJR BRA, TO L1
LSET L2 ( lblval ) TO? () TST, IFZ, ( read )
  [S+0] LDD, S+0 STD, ELSE, ( write )
  2 S+N LDD, [S++] STD, S++ TST, THEN, SYNC,
LSET L3 ( VALUE ) L2 () JSR, $1234 M,
L1 FMARK $ff # LDS,
( read ) \ TO? () STS,
( write ) 42 # LDD, PSHS, D TO? () STB,
L3 BR BRA,
'HERE !
