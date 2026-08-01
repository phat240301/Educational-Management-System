# Contributing Guide

## Repository Structure

```
github
├── develop
├── main
├── feature/*
├── release/*
└── hotfix/*
```

- **`main`** — production-ready code only. Every commit on `main` is deployable.
- **`develop`** — integration branch. All finished features land here first.
- **`feature/*`** — one branch per feature/task, branched from `develop`, merged back into `develop` via PR.
- **`release/*`** — cut from `develop` when preparing a release; only bug fixes/polish happen here before merging into both `main` and `develop`.
- **`hotfix/*`** — urgent fixes branched from `main`, merged back into both `main` and `develop`.

## Git Flow

```
feature
    \
develop -------> release -----> main
                    ^
                    |
                 hotfix
```

## Workflow

1. **New feature**
   ```
   git checkout develop
   git pull
   git checkout -b feature/<short-description>
   # work, commit
   git push -u origin feature/<short-description>
   ```
   Open a PR into `develop`. CI must pass before merging.

2. **Preparing a release**
   ```
   git checkout develop
   git pull
   git checkout -b release/<version>
   # only bugfixes/version bump/docs here
   ```
   Merge `release/<version>` into `main` (tag it, e.g. `v1.2.0`) **and** back into `develop`.

3. **Hotfix**
   ```
   git checkout main
   git pull
   git checkout -b hotfix/<short-description>
   # fix, commit
   ```
   Merge into `main` (tag a patch version) **and** back into `develop`.

## Commit & PR conventions

- Keep commits focused; write the *why* in the message, not just the *what*.
- PRs must pass CI (build, test, Docker image build) before merge.
- Prefer squash-merge for `feature/*` → `develop` to keep history readable.
