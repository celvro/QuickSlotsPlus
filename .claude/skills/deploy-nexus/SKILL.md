---
name: deploy-nexus
description: Cut and publish a QuickSlotsPlus release to Nexus Mods (Subnautica and Below Zero) - changelog, README, version bump, build, zip, upload, mod page version and description. Use when asked to deploy, publish, release or upload the mod to Nexus / NexusMods, or to cut a release.
---

# Release to Nexus Mods

A release is: write the changelog, update the README, bump the version, zip and upload the file,
post the changelog, bump the version on the mod page, and update the mod page description if the
mod's behaviour changed. `Deploy-NexusMods.ps1` in this folder does the mechanical parts; the
writing is yours, and so is the description, which you edit in the user's Chrome. Run the script with the PowerShell tool from the repo root.

| Step | Who does it |
| --- | --- |
| Write the changelog, update README feature text | You, step 1 |
| Bump version in code and manifest, stamp the changelog heading | Script, `-SetVersion` |
| Build both games, zip | Script, every run |
| Upload file, post changelog, bump mod page version | Script, `-Publish` |
| Mod page description | You, in the user's Chrome (no API for it), step 6 |

Publishing is public and cannot be cleanly undone. Always dry-run first and get the user's
go-ahead before `-Publish`.

## One-time setup

1. **API key.** The user creates one at https://www.nexusmods.com/settings/api-keys and stores it
   themselves as the `NEXUSMODS_API_KEY` user environment variable (`setx NEXUSMODS_API_KEY <key>`).
   The script reads process scope, then user scope, so no restart is needed. Never ask for the key
   in chat, print it, or write it to a file.
2. **IDs in `nexus.json`.** `modId` is the number in the mod page URL. `fileId` is the v3 mod file
   ID, which is not the number in download links; get it with `-ListFiles` (needs the key) and
   paste the `fileId` of the main file. A game with an empty `fileId` is skipped under `-Game All`.

The API can only add versions to a file that already exists on a mod page. It cannot create a mod
page or a first file; those are done once on the website.

## Release steps

1. **Changelog and README.** Find the last release commit (`git log --oneline --grep "^Release"`)
   and read what changed since. In `README.md`:
   - Make sure `### Unreleased` under `## Changes` has a bullet for every user-visible change,
     written for players (what they will notice), not as commit messages. Leave out internal
     refactors and diagnostics.
   - Update the body where behaviour changed: Features, Requirements (minimum Nautilus version),
     options, Nitrox notes, Known issues.
   - Make the same body edits in `release\README.md`, the shorter store readme. It is gitignored
     and hand-maintained, so edit it in place; do not copy `README.md` over it.

   Show the user the changelog and agree the version number (patch for fixes, minor for features)
   before going on.

2. **Bump.** This writes the version into `QuickSlotsPlus.cs` and `release\manifest.json`, renames
   `### Unreleased` to the version heading, and copies that section into `release\README.md`:

   ```
   .\.claude\skills\deploy-nexus\Deploy-NexusMods.ps1 -SetVersion 3.2.0 -Title "Short name"
   ```

   `-Title` is optional and gives `### 3.2.0 (Short name)`.

3. **Commit on main.** Commit as `Release <version>`. The script refuses to publish from a dirty
   tree or a branch other than `main` unless `-Force` is passed; pass it only when the user says so.
   If the work is on another branch, ask how they want it merged rather than forcing.

4. **Dry run**, then show the user the version, changelog and zip sizes it printed:

   ```
   .\.claude\skills\deploy-nexus\Deploy-NexusMods.ps1
   ```

5. **Publish** after the user confirms. This uploads each zip as a new version of the existing
   file, archives the previous one, sets the mod page version, and posts the changelog:

   ```
   .\.claude\skills\deploy-nexus\Deploy-NexusMods.ps1 -Publish
   ```

6. **Mod page description.** The API has no endpoint for it, so edit the page in the user's Chrome
   with the Claude in Chrome tools (`mcp__claude-in-chrome__*`). See "Editing the description"
   below. Skip this step, and say so, when nothing a player reads changed: diff the README body
   against the last release (`git diff <last release commit> -- README.md`, ignoring `## Changes`
   and `## Build from source`).

7. **Wrap up.** Report the version IDs and file-tab links the script printed. Offer to tag the
   commit (`v<version>`) and push; do not do either unasked.

## Editing the description

This runs in the user's real Chrome, where they are signed in to Nexus Mods. A headless or
Playwright-launched browser does not work: Nexus puts a bot check in front of automated browsers,
and that check is not to be worked around. If the extension is not connected, or the page says
"Your session has expired", ask the user to connect it or sign in, and wait.

The page is `https://www.nexusmods.com/games/<gameDomain>/mods/<modId>/edit/general` (values from
`nexus.json`), verified November 2026. Open it in a new tab. The description is an SCEditor
instance holding BBCode; drive it through its own API with `javascript_tool`, not by typing:

```js
const editor = window.sceditor.instance(document.querySelector('.bbcode-editor > textarea'));
editor.val();          // read the BBCode
editor.val(newText);   // write it; the page notices and enables Save
```

1. **Read** the current text. Tool output is cut at about 1000 characters, so read it in slices
   (`editor.val().slice(0, 900)`, `.slice(900, 1800)`, ...). Read `editor.val()`, not the
   textarea's `value`: the editor normalises BBCode, so the two differ slightly.
2. **Plan the edits** as exact replacements of the passages that are out of date, in the page's
   existing BBCode style (`[size=4]Heading:[/size]`, `* ` bullets, `[url=..]..[/url]`,
   `[quote][code]..[/code][/quote]`). Do not rewrite the whole description from the README: the
   page has its own wording, thanks and Discord link that the README lacks. The text contains
   zero-width characters, so copy the passage to replace from what you read, never retype it.
3. **Show the user** each passage before and after, and get their OK. This is public content.
4. **Apply** each replacement in the page, and check it matched exactly once before writing:

   ```js
   const before = editor.val();
   if (before.split(oldText).length !== 2) throw new Error('passage not found exactly once');
   editor.val(before.replace(oldText, newText));
   ```

5. **Save** with the page's `Save` button (there are two, top bar and footer; either works), then
   reload the tab and read the text back to confirm the change is there. Close the tab when done.

Other fields on the same page, if they need changing: `#short-description` (the one-line summary)
and `#mod-version` (already set by the upload when `updateModVersion` is true).

Once the editor has been written to, `Save` stays enabled even if the text is put back, and
closing or reloading the tab without saving may raise a "Leave site?" dialog that stalls the
extension. If you need to abandon an edit, tell the user to close the tab themselves.

If `window.sceditor` or the `.bbcode-editor` textarea is missing, Nexus has changed the form.
Find the description field again with `read_page` before going further, and update this section.

## Options

| Flag | Effect |
| --- | --- |
| `-SetVersion X.Y.Z [-Title ".."]` | Prepare step 2 only. Builds and uploads nothing. |
| `-Game Subnautica\|BelowZero\|All` | Which game to release. Default `All`. |
| `-ListFiles` | Print each configured mod page's files and their `fileId`s, then exit. |
| `-SkipBuild` | Zip what is already in `release\<Game>\` instead of rebuilding. |
| `-Changelog "a`nb"` | Changelog text, one entry per line, instead of the README section. |
| `-NoChangelog` | Upload the file without touching the mod page changelog. |
| `-Force` | Publish despite a dirty tree or non-`main` branch. |

Per-game behaviour is in `nexus.json`: `archiveExisting` moves the previous version to the archive,
`updateModVersion` sets the mod page version to the uploaded one, `category` is `main`, `optional`
or `miscellaneous`.

## When it fails

- **`CS0117` in `LabelUtil.cs`** is a stale NuGet restore for the other game, not a missing API;
  see the Build section of `CLAUDE.md`.
- **Build passes but "DLL was not refreshed"**: the post-build copy into `release\` did not run.
- **401/403 from the API**: the key is wrong, or the account is not an author on that mod page.
- **404 on `/mod-files/<id>/versions`**: `fileId` is the legacy numeric ID; use `-ListFiles`.
- **A failure after "Uploading"** but before "published version id" leaves nothing on the mod
  page, so re-running is safe. If Subnautica published and Below Zero then failed, re-run with
  `-Game BelowZero -SkipBuild -NoChangelog` when both games share one mod page, so Subnautica and
  the changelog are not posted twice.
- The build installs into the game folder as a side effect, and skips that while the game runs.
- A dry run overwrites `release\<Game>\<zip for the current version>`. Bump first if that zip is
  one you want to keep.

The upload API is in open beta. If requests start failing with shape errors, compare against the
reference client at https://github.com/Nexus-Mods/upload-action (`src/index.ts`).
