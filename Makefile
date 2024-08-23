run-dev:
	cd server/src/WebAPI && dotnet build
	podman build ./server -t inkless-dev

	podman pod create \
	--name inkless \
	-p 5432:5432 \
	-p 8080:8080 \
	--replace

	podman run \
	--pod inkless \
    -d \
    -v inkless-database:/var/lib/postgresql/data:Z \
    -e POSTGRES_DB=inkless \
    -e POSTGRES_USER=postgres \
    -e POSTGRES_PASSWORD=1234 \
    --name inkless-postgres \
    --replace \
    postgres:16.3

	podman run \
	--pod inkless \
	-d \
	-e ASPNETCORE_ENVIRONMENT=Development \
    --name inkless-dev \
    --replace \
    inkless-dev

	cd client && bun install
	cd client && bun run dev

test:
	cd server && dotnet test

test-count:
	cd server && dotnet test -t --verbosity=quiet --nologo | grep "^    " | wc -l | xargs echo "Total: $1"
