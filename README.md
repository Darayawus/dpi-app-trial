# PrintBucket

**Trial version — 0.0.0**  
This repository contains a trial implementation of PrintBucket intended to evaluate feasibility. The project is work-in-progress and many features, hardening and production tasks remain to be completed.

Summary
-------
PrintBucket is an application composed of a Razor Pages UI (`PrintBucket.Web`), a REST API (`PrintBucket.Api`) and AWS helper libraries (`PrintBucket.AWS`). 
It lets users create logical buckets, upload images (multiple versions) to S3 and store metadata in DynamoDB.

Demo
----
- Web UI: https://printbucket.darioparres.com
- API: https://printbucket.darioparres.com/api

Repository layout
-----------------
- `src/PrintBucket.Web` — Razor Pages frontend.
- `src/PrintBucket.Api` — REST API (upload, buckets, endpoints).
- `src/PrintBucket.AWS` — AWS services (S3, DynamoDB, utilities).
- `src/PrintBucket.Models` — shared models (Bucket, ImageRecord).
- `src/PrintBucket.Common` — common utilities (logging, etc.).
- `src/PrintBucket.Tests` — unit / integration tests.
- `docs/` — docs files.

Requirements
------------
- .NET 8 SDK
- Native `libvips` (required by `NetVips`) for image processing
- AWS credentials (profile, environment variables or IAM role)

Local setup
-----------
1. Clone and restore:
   - `git clone <repo>`
   - `dotnet restore`
2. AWS credentials:
   - Use `~/.aws/credentials` profile or environment variables `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`.
   - Optionally set `AWS:Profile` in `PrintBucket.Api/appsettings.Development.json`.
3. Configuration:
   - Set S3 and DynamoDB table names in `appsettings.Development.json`:

4. DynamoDB:
   - Create `dpi_bucket` (partition `hash_key`, sort `range_key`) or adapt `BucketService`.
   - Create `dpi_files` (e.g. partition `bucketId`, sort `id`) or adapt `ImageService`.

Run locally
-----------
- From IDE: use the launch profile in `Properties/launchSettings.json`.
- From terminal:
  - API: `dotnet run --project src/PrintBucket.Api`
  - Web: `dotnet run --project src/PrintBucket.Web`

Tests
-----
- Run tests: `dotnet test`


API Documentation & Monitoring
----------------------------

### Swagger
The API includes Swagger/OpenAPI documentation, available in development mode at:
- https://localhost:5000/swagger (when running locally)

### Metrics
Both the Web UI and API expose Prometheus-compatible metrics endpoints:

- Web UI metrics: 
  - Local: http://localhost:5000/metrics
  - Production: https://printbucket.darioparres.com/metrics

- API metrics:
  - Local: http://localhost:5003/metrics
  - Production: https://printbucket.darioparres.com/api/metrics

Available metrics include:
- HTTP request duration
- Request counts by endpoint
- Response status codes
- Active connections
- Runtime metrics (GC, thread pool, etc.)

These endpoints can be scraped by Prometheus and visualized using Grafana dashboards.

## Continuous Integration (CI) / Continuous Deployment (CD)

I use an automated CI/CD pipeline to build, test and produce artifacts for PrintBucket. The canonical CI/CD server for this repository is:

https://buildserver.parresibarra.com

If needed a readonly access can be provided.

### TeamCity Environment

Project Overview:
![TeamCity Projects](docs/img/teamcity_01.png)

Build Configuration:
![Build Steps](docs/img/teamcity_02.png)

Build Steps Detail:
![Build Configuration](docs/img/teamcity_03.png)

Build History and Artifacts:
![Build History](docs/img/teamcity_04.png)

How it works
- On push to `main` and on pull requests the pipeline restores, builds and runs tests for all projects.
- After a successful pipeline the CI server may be notified to run further steps (packaging, deployment).
- CI secrets (tokens or webhook URLs) must be stored in your CI provider.
- CI version number is shown in the footer of the Web UI.


Deployment notes
----------------
- Configure S3 bucket policies for public access. Avoid relying on ACLs if bucket has ACLs disabled.
- Store AWS credentials securely in CI/CD (secrets manager / env vars).

Operational notes
-----------------
- Install native `libvips` on hosts used for image processing.
- Serilog is configured in `PrintBucket.Common.Logging`; adjust sinks as needed.

## Why PrintBucket?

### Value Proposition
- **Social experience:** event attendees can upload their photos to a **shared album**.
- **Simplicity:** sharing via **QR code** avoids the usual chaos of exchanging photos over WhatsApp or social media.
- **AI personalization:** smart analysis/selection provides an edge over conventional galleries.
- **Natural monetization:** enables **photo and album printing**.

### Features that can strengthen the product (roadmap)
- **Collaborative editing:** multiple people contribute photos to the same album.
- **AI auto-curation:** drop blurred shots, closed eyes, and duplicates; prioritize the “best” photos.
- **Album templates:** ready-made designs for printing or **PDF** export.
- **Privacy options:** album **public/private**, password-protected, or **time-limited access**.

> **Proposed QR flow:** organizer creates album → generates **QR** → guests scan and upload photos → creator reviews/curates → optionally **prints** or exports.

### Highlighted use cases
- **Weddings:** QR on the tables; guests upload their photos; the couple gets everything neatly organized.
- **Birthdays & communions:** parents collect everyone’s photos without chasing chat messages.
- **Group trips:** each traveler contributes, creating a shared trip album.
- **Corporate events:** shared galleries for trade shows, launches, team buildings.

### Short roadmap
- [ ] Generate **QR** per album (view + endpoint).
- [ ] **Guest** access with time-limited code (TTL).
- [ ] Basic curation (blur detection / top-N selection).
- [ ] **PDF** export using a template.
- [ ] Admin/guest roles surfaced in the UI.




