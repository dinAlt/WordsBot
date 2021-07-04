#!/bin/sh
set -e
DB=/data/base.db
BACKUP=/data/base.backup

[ -f $DB ] && rm -f $BACKUP && cp $DB $BACKUP
chown -R wordsbot:wordsbot /data
gosu wordsbot dotnet WordsBot.dll