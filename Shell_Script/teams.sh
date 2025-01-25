#!/bin/sh

t_file="`cat teams.dat | tr -d " "`"
h_file="`cat hallgato.dat | tr -d " "`"
if test $1 = "-lista"
then
echo "$2 $3 a következő kurzus(oka)t tartja: "
for sor in $t_file
    do
	if test $2$3 = "`echo "$sor" | cut -d "," -f3`"
	then
	echo "$sor" | cut -d "," -f1
	fi
    done
elif test $1 = "-hallgato"
then
for h_sor in $h_file
    do
	if test $2$3 = "`echo "$h_sor" | cut -d "," -f1`"
	then
	adatok="`echo "$h_sor" | cut -d "," -f2-`"
	for t_sor in $t_file
	    do
		kod="`echo "$t_sor" | cut -d "," -f2`"
		if test -n "`echo "$adatok" | grep -o "$kod"`"
		then
		nevek="$nevek`cat teams.dat | grep "$kod" | cut -d "," -f3 | cut -c2-`\n"
		fi
	    done
	echo "$2 $3-t az alábbi tanár(ok) tanítják:"
	echo "$nevek" | head -n-1 | sort -u
	fi
    done
elif test $1 = "-sok"
then
oktato="`cat teams.dat | cut -d "," -f3 | cut -c2- | sort`"
oktato_sort="`echo "$oktato" | sort -u`"
oktato_unsort="`echo "$oktato" | tr -d " "`"
max=0
for nev in $oktato_sort
    do
	nev="`echo $nev | tr -d " "`"
	if test $max -lt `echo $oktato_unsort | grep -o "$nev" | wc -l`
	then
	max=`echo $oktato_unsort | grep -o "$nev" | wc -l`
	neve="`cat teams.dat | cut -d "," -f3 | cut -c2- | grep $nev | head -n1`"
	fi
    done
echo "$neve-nak/nek van a legtöbb meghirdetett kurzusa."
fi