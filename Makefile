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

run-db:
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

publish:
	cd client && bun install
	cd client && bun run build
	cp -rfT client/dist server/src/WebAPI/wwwroot
	cd server/src/WebAPI && dotnet publish -c Release

test:
	cd server && dotnet test

test-count:
	cd server && dotnet test -t --verbosity=quiet --nologo | grep "^    " | wc -l | xargs echo "Total: $1"
