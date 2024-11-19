#!/bin/bash

set -e

cd "`dirname "$0"`"

FILES=`ls | grep '\.fx$'`
for f in $FILES
do
	WINEDEBUG=fixme-all,err-all wine fxc.exe /T fx_2_0 $f /Fo "`basename $f .fx`.fxb"
done
