FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/WordsBot/*.csproj ./src/WordsBot/
COPY src/WordsBot.Common/*.csproj ./src/WordsBot.Common/
COPY src/WordsBot.Database.Sqlite/*.csproj ./src/WordsBot.Database.Sqlite/
COPY src/WordsBot.Translators.YandexTranslate/*.csproj ./src/WordsBot.Translators.YandexTranslate/
COPY tests/WordsBot.Common.Test/*.csproj ./tests/WordsBot.Common.Test/
COPY tests/WordsBot.Translators.YandexTranslateTest/*.csproj ./tests/WordsBot.Translators.YandexTranslateTest/
COPY *.sln .
RUN dotnet restore
# copy and publish app and libraries
COPY . .
RUN cd src/WordsBot && dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime
ENV DEBIAN_FRONTEND=noninteractive

RUN apt update && apt install gosu && apt purge -y --autoremove && rm /var/lib/apt/lists -rf
RUN groupadd -r wordsbot --gid=1000 && \
  useradd -r -g wordsbot --uid=1000 --home-dir=/app wordsbot

WORKDIR /app

COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
COPY --from=build /app .

ENTRYPOINT ["/app/entrypoint.sh"]