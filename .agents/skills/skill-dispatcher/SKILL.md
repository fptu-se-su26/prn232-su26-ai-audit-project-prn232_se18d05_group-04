---
name: skill-dispatcher
description: Analyze user requests, scan .agents/skills/, select matching skills, and justify each choice. Also enforces core engineering discipline: think before coding, simplicity first, surgical changes, goal-driven execution.
---

# Skill Dispatcher

## 1. Core Engineering Discipline

These rules apply to every task, before any skill selection.

### 1.1 Think Before Coding

**Do not assume. Do not hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them — do not pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what is confusing. Ask.

### 1.2 Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that was not requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

### 1.3 Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Do not "improve" adjacent code, comments, or formatting.
- Do not refactor things that are not broken.
- Match existing style, even if you would do it differently.
- If you notice unrelated dead code, mention it — do not delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Do not remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

### 1.4 Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

## 2. Request Analysis & Skill Dispatch

**Before any implementation, run this dispatch protocol.**

### 2.1 Analyze the Request

Extract these signals from the user's request:

| Signal | What to look for |
|--------|-----------------|
| **Domain** | Frontend UI? Backend logic? Database? DevOps? Documentation? |
| **Action** | Build new? Fix bug? Refactor? Review? Design? Research? |
| **Complexity** | Single file? Multi-file? New feature? Architecture change? |
| **Visual requirement** | Layout/styling? Animation/motion? Image generation? Design reference? |
| **Tech stack hints** | React? Next.js? GSAP? Tailwind? Node.js? |

### 2.2 Scan Available Skills

Read the directory listing of `.agents/skills/` to discover all available skills. For each skill found, read its `SKILL.md` frontmatter (`name` and `description` fields) to understand what it covers.

The skills live at `.agents/skills/<skill-name>/SKILL.md`.

### 2.3 Match & Select

Compare request signals against each skill's description. A skill matches when:
- The request domain overlaps with the skill's expertise area
- The action type fits the skill's designed purpose
- Using the skill would change the approach (not just add overhead)

**Selection rules:**
- Zero matches → proceed with core discipline only, state that no specialized skill fits
- One match → use that skill
- Multiple matches → use all that apply, ordered by priority (process skills first, then domain skills)
- When in doubt between two similar skills, state the tradeoff and pick the more specific one

### 2.4 Justify Each Selection

For every skill selected, output a brief justification block:

```
[skill-name] → selected because: [1-2 sentence reason tied to specific request signals]
```

For skills considered but NOT selected, output:

```
[skill-name] → skipped because: [1 sentence on why it does not fit]
```

### 2.5 Skill Catalog Reference

The `.agents/skills/` directory may contain any number of skills. Below is a reference of the currently known skills. This catalog may become stale — always scan the directory at dispatch time to discover new or removed skills.

#### design-taste-frontend
- **Domain:** Frontend UI/UX design & implementation
- **Best for:** Building new UI components, pages, or full interfaces. Enforces anti-AI-slop rules, premium design patterns, Tailwind, React/Next.js. Covers typography, color calibration, layout diversification, glassmorphism, bento grids, motion physics.
- **Not for:** Backend logic, database work, image generation, pure code review
- **Key signals:** "UI", "component", "page", "design", "layout", "style", "frontend", "Tailwind", "React component", "dashboard"

#### gpt-taste
- **Domain:** Elite AWWWARDS-level frontend with GSAP animation
- **Best for:** Landing pages, marketing sites, scroll-driven storytelling. Uses Python-driven randomization for layout variance, AIDA structure, gapless bento grids, ScrollTrigger pinning/stacking/scrubbing. Heavier on animation than design-taste-frontend.
- **Not for:** Simple UI tweaks, backend work, dashboards without heavy animation
- **Key signals:** "landing page", "animation", "GSAP", "scroll", "marketing site", "AWWWARDS", "creative", "cinematic", "storytelling"

#### imagegen-frontend-web
- **Domain:** Image generation for frontend design references
- **Best for:** Generating design mockups, section-by-section website previews, visual references for developers. Produces horizontal images per section with strict composition variety rules.
- **Not for:** Writing code, implementing UI, fixing bugs
- **Key signals:** "mockup", "design reference", "preview image", "website concept", "visual comp", "generate image"

---

## 3. Dispatch Workflow

Full workflow for every user request:

```
1. ANALYZE — extract domain, action, complexity, visual requirements, tech stack
2. SCAN — list .agents/skills/ directory, read frontmatter of each SKILL.md
3. MATCH — compare signals against each skill's description
4. JUSTIFY — output selection block (selected + skipped with reasons)
5. EXECUTE — load selected skills and follow their instructions, governed by core discipline rules (§1)
```

### 3.1 Quick Dispatch (trivial tasks)

For trivial tasks (typo fix, single-line change, simple question), run a fast-path:
- Skim the request for domain signals (2 seconds)
- If no skill obviously applies, skip the full scan and proceed with core discipline only
- State: "No skill dispatch needed — trivial task."

Trivial means: one file, one line, no design decision, no architecture change.

### 3.2 Example Dispatch

**User request:** "Build a hero section for a fintech landing page with scroll animations"

```
ANALYZE:
  Domain: Frontend UI
  Action: Build new
  Complexity: Multi-file (component + styles + animation)
  Visual: Layout, animation, design reference
  Stack: React/Next.js + GSAP

SCAN: 3 skills found in .agents/skills/

MATCH:
  design-taste-frontend → matches: frontend UI, hero section design, anti-slop rules
  gpt-taste → matches: landing page, scroll animation, GSAP, AIDA structure
  imagegen-frontend-web → matches: could generate design reference first

SELECT:
  [design-taste-frontend] → selected because: core UI implementation with anti-slop rules for hero layout, typography, and color
  [gpt-taste] → selected because: GSAP scroll animations, AIDA page structure, and hero architecture randomization are direct matches for "landing page with scroll animations"
  [imagegen-frontend-web] → skipped because: request asks for code implementation, not image reference generation
```

---

## 4. Skill Interaction Rules

When multiple skills are selected:

1. **Process skills run first** — they set the approach
2. **Domain skills run next** — they carry out the implementation
3. **Conflicts** — if two skills give contradictory instructions, prefer the more specific skill for the task at hand. State the conflict and your resolution.
4. **Overlap** — when two skills cover the same ground (e.g., both have anti-slop rules), apply both but deduplicate. The stricter rule wins.

---

## 5. Post-Execution Change Report

After completing the task, report all changes made. This must be part of every response after execution.

**Report format:**

```
## Changes made:

File | Action | Detail
--- | --- | ---
path/to/file | created/modified/deleted | What changed and why (1-2 sentences)
...
```

For multi-file changes, list every file touched. For single-file changes, a one-line summary is enough.

## 6. Post-Execution Verification

After completing the task and reporting changes, verify against core discipline:

- [ ] Did you state assumptions before coding?
- [ ] Is the solution the simplest possible?
- [ ] Did you touch only what was necessary?
- [ ] Did you define and meet success criteria?
- [ ] Did you follow each selected skill's instructions?
- [ ] Were skill selections justified and correct in hindsight?
