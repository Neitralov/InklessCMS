version ?= latest

run:
	make -j 2 run-backend run-frontend

run-backend:
	podman run \
    -d \
    -p 5432:5432 \
    -v inkless-database:/var/lib/postgresql/data:Z \
    -e POSTGRES_DB=inkless \
    -e POSTGRES_USER=postgres \
    -e POSTGRES_PASSWORD=1234 \
    --name inkless-postgres \
    --replace \
    postgres:16.3

	dotnet run --project server/src/WebAPI

run-frontend:
	cd client && bun install
	cd client && bun run dev

publish-and-build-docker-images: publish build-docker-images

publish: publish-inkless publish-database-migrator

publish-inkless:
	cd client && bun install
	cd client && bun run build
	cp -rfT client/dist server/src/WebAPI/wwwroot
	cd server/src/WebAPI && dotnet publish -c Release

publish-database-migrator:
	cd server/src/Database.Migrator && dotnet publish -c Release

build-docker-images: build-inkless-docker-image build-database-migrator-docker-image

build-inkless-docker-image:
	podman build ./server/src/WebAPI -t docker.io/neitralov/inkless:$(version)

build-database-migrator-docker-image:
	podman build ./server/src/Database.Migrator -t docker.io/neitralov/inkless-migrator:$(version)

add-migration:
	dotnet ef migrations add $(migration-name) --startup-project server/src/WebAPI --project server/src/Database.Migrator/

test:
	cd server && dotnet test

test-count:
	cd server && dotnet test -t --verbosity=quiet --nologo | grep "^    " | wc -l | xargs echo "Total: $1"
