: (emit) 0 PC! ;
390 LOAD  ( forth high )
(entry) _
( Update LATEST )
PC ORG @ 8 + !
," BLK$ "
," ' EFS@ ' BLK@* **! "
," ' EFS! ' BLK!* **! "
EOT,
