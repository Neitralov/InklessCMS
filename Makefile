version ?= latest

run:
	dotnet run --project src/WebAPI

run-db:
	docker run \
    -d \
    -p 5432:5432 \
    -v inkless-database:/var/lib/postgresql/data:Z \
    -e POSTGRES_DB=inkless \
    -e POSTGRES_USER=postgres \
    -e POSTGRES_PASSWORD=1234 \
    --name inkless-postgres \
    postgres:16.9

run-migrator:
	dotnet run --project src/Database.Migrator

publish-and-build-docker-images: publish build-docker-images

publish: publish-inkless publish-database-migrator

publish-inkless:
	cd src/WebAPI && dotnet publish -c Release

publish-database-migrator:
	cd src/Database.Migrator && dotnet publish -c Release

build-docker-images: build-inkless-docker-image build-database-migrator-docker-image

build-inkless-docker-image:
	docker build ./src/WebAPI -t docker.io/neitralov/inkless:$(version)

build-database-migrator-docker-image:
	docker build ./src/Database.Migrator -t docker.io/neitralov/inkless-migrator:$(version)

add-migration:
	dotnet ef migrations add $(migration-name) --startup-project src/WebAPI --project src/Database.Migrator/
