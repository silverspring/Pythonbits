#! /bin/sh
PREFIX=/usr/local

while getopts ":p:" Option
do
    case $Option in
        p) PREFIX=$OPTARG
    esac
done

INSTALLDIR=$PREFIX/bin
echo "Installing to $INSTALLDIR"

cp BeautifulSoup.py $INSTALLDIR
cp MultipartPostHandler.py $INSTALLDIR
cp microdata.py $INSTALLDIR
cp pythonbits.py $INSTALLDIR/pythonbits
chmod o+x $INSTALLDIR/pythonbits
