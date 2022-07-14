# Words bot.

Small single user telegram bot for English to Russian translation and words training. Built with `.Net 6.0`, `SQLite`, `EF Core`, `DI` and `Yandex Translate API`. You can try it [here](https://t.me/words_ru_en_game_bot).

## Before start

You need a Docker or a dotnet framework 6.0 installed to build this app, and also a docker-compose if you want to run it via compose file.

## Build

### With docker

```sh
git clone https://github.com/dinAlt/WordsBot.git
cd WordsBot
docker build .
```

### With dotnet CLI

```sh
git clone https://github.com/dinAlt/WordsBot.git
cd WordsBot
dotnet build --configuration Release

```

See [dotnet documentation](https://docs.microsoft.com/dotnet/core/tools/dotnet-build) for more build options.

You can also youse [dotnet publish](https://docs.microsoft.com/dotnet/core/tools/dotnet-publish) command to build portable versions of executable.

## Run

### With docker compose

```sh
mv env_sample .env
```

Then open `.env` file in text editor and set variables to values you got from `@BotFather` bot and `Yandex Translate API` (but do not touch `WORDSBOT_SqliteDatabasePath` variable).

```sh
# for all docker-compose versions
docker-compose up
# newer command for docker-compose v2
docker compose up
```

### Executable built with `dotnet build`

Tested on Linux, on Windows paths and commands may vary.

```sh
cd src/WordsBot/bin/Release/net6.0
mv config.sample.json config.json
```

Then open config.json with text editor and set options to values you got from `@BotFather` bot and `Yandex Translate API`.

```sh
./WordsBot
# probably, WordsBot.exe on Windows
```
