#!/bin/sh
set -e

chown -R wordsbot:wordsbot /data
gosu wordsbot dotnet WordsBot.dll