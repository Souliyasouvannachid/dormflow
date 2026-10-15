---
name: DormFlow
description: A calm working desk for daily dormitory administration in Thai.
colors:
  primary: "#216650"
  primary-deep: "#194b3c"
  navigation: "#193f35"
  canvas: "#f5f6f2"
  surface: "#ffffff"
  ink: "#1f302a"
  muted: "#5b6c64"
  border: "#dde3da"
  field-border: "#c7d2ca"
  focus: "#bc6d20"
  positive: "#347d57"
  pending: "#b78331"
  occupied: "#235a48"
  urgent: "#b14b38"
  maintenance: "#98613c"
  status-neutral: "#8a9d91"
  error: "#ac392c"
  chart-expense: "#d6a459"
typography:
  headline:
    fontFamily: "Noto Sans Thai, sans-serif"
    fontSize: "30px"
    fontWeight: 600
    lineHeight: 1.5
  title:
    fontFamily: "Noto Sans Thai, sans-serif"
    fontSize: "18px"
    fontWeight: 600
    lineHeight: 1.5
  body:
    fontFamily: "Noto Sans Thai, sans-serif"
    fontSize: "15px"
    fontWeight: 400
    lineHeight: 1.5
  label:
    fontFamily: "Noto Sans Thai, sans-serif"
    fontSize: "14px"
    fontWeight: 500
    lineHeight: 1.5
  metric:
    fontFamily: "Manrope, sans-serif"
    fontSize: "26px"
    fontWeight: 600
    lineHeight: 1.6
rounded:
  navigation: "6px"
  control: "7px"
  panel-mobile: "10px"
  panel: "12px"
spacing:
  xs: "4px"
  sm: "8px"
  md: "12px"
  lg: "16px"
  xl: "24px"
  2xl: "32px"
  3xl: "48px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.surface}"
    rounded: "{rounded.control}"
    padding: "10px 16px"
  button-primary-hover:
    backgroundColor: "{colors.primary-deep}"
  button-outline-primary:
    textColor: "{colors.primary}"
    rounded: "{rounded.control}"
    padding: "10px 16px"
  button-outline-secondary:
    textColor: "#53665b"
    rounded: "{rounded.control}"
    padding: "10px 16px"
  button-link:
    textColor: "{colors.primary}"
    rounded: "{rounded.control}"
    padding: "10px 0"
  field:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    rounded: "{rounded.control}"
  panel:
    backgroundColor: "{colors.surface}"
    rounded: "{rounded.panel}"
    padding: "24px"
  navigation-item:
    textColor: "#e0eae3"
    rounded: "{rounded.navigation}"
    padding: "11px 14px"
  navigation-item-active:
    backgroundColor: "{colors.canvas}"
    textColor: "{colors.navigation}"
  status:
    textColor: "#50625a"
---

# Design System: DormFlow

## Overview

**Creative North Star: "The Working Desk"**

DormFlow uses the working-desk direction already established for daily administration. Warm off-white surroundings, forest-green navigation and white working surfaces support dense, readable Thai forms and tables. The interface is quiet and practical, with clear actions and restrained visual emphasis.

Persistent desktop navigation and a compact expandable mobile menu preserve the same operational structure. Figures use a distinct numeral face, while status text accompanies colored dots. This document records the implemented system in web/src/styles.css, Forms.jsx and App.jsx; frontmatter is normative for the reusable values it contains.

**Key Characteristics:**
- Thai-first labels and self-hosted interface fonts.
- Forest-green navigation with warm white working surfaces.
- Bordered panels and precise table dividers without decorative shadows.
- Text-backed status colors and conspicuous keyboard focus.
- Responsive layouts, reduced-motion support and printable details.

## Colors

Forest greens and warm neutrals establish the workspace; amber and rust communicate state and distinguish chart series.

### Primary
- **Forest Green** (primary): primary actions, links, input caret, focused field border and revenue series.
- **Deep Forest** (primary-deep): primary hover and active states.
- **Forest Shade** (navigation): navigation, occupancy figures and emphasized totals.

### Secondary
- **Harvest Amber** (chart-expense): expense bars and a payment-method chart segment. The monthly line chart currently uses a darker amber directly in its chart options.
- **Focus Copper** (focus): keyboard focus outlines.

### Tertiary
- **Positive Green** (positive): available, active, paid, confirmed and completed dots.
- **Pending Amber** (pending): reserved, pending, partially paid and received dots.
- **Occupied Green** (occupied): occupied-room dot.
- **Urgent Rust** (urgent): overdue and urgent dots.
- **Maintenance Brown** (maintenance): maintenance and reported dots.
- **Neutral Sage** (status-neutral): fallback status dot.
- **Error Red** (error): inline validation.

### Neutral
- **Warm Paper** (canvas): workspace and active navigation surface.
- **White** (surface): panels, fields and top bar.
- **Deep Ink** (ink): main text.
- **Quiet Sage** (muted): supporting text and table headings.
- **Panel Rule** (border): main panel and top-bar borders.
- **Field Rule** (field-border): field strokes.

**The Text and Color Rule.** Status indicators always retain a readable state label beside the colored dot.

## Typography

**Body and heading font:** self-hosted Noto Sans Thai, sans-serif; regular, medium and semibold files are loaded.
**Numeral and wordmark font:** self-hosted Manrope, sans-serif; the semibold file is loaded.

The Thai face provides the shared interface voice; Manrope distinguishes the wordmark and prominent figures.

### Hierarchy
- **Headline:** the headline role for page titles, reducing to (25px) on mobile. Login editorial text has its own responsive (30px–47px) treatment with (1.7) line height.
- **Title:** the title role for panel headings; compact section headings reduce to (16px) on mobile.
- **Body:** the body role with Bootstrap's inherited line height. Paragraphs use (1.8) line height. Supporting page copy is (14px), tables (13px), metadata generally (12px).
- **Label:** the label role for visible form labels, with (8px) separation from controls.
- **Metric:** the metric role for financial figures, growing to (32px) on wide screens and reducing to (21px) on mobile. Occupancy uses (48px), reducing to (40px) on mobile.

**The Comparable Figures Rule.** Financial metrics and table values use tabular numerals so amounts remain easy to compare.

## Layout

Desktop uses a fixed (238px) navigation column, a (73px) top bar and main padding of (32px 36px 48px). Main content caps at (1600px) on wide screens. The spacing scale is a reusable rhythm; local gaps also use intermediate values.

The dashboard places four metrics in one divided surface and uses a two-column content grid with a (24px) gap and (1.5:1) ratio, becoming (1.7:1) on wide screens. Editors cap at (960px) with two form columns and (24px) gaps. Detail fields use three columns. Table cells use (15px 20px) padding; tables scroll horizontally inside their container. Search fields cap at (400px) on desktop.

At (1150px) and below, navigation narrows to (212px), dashboard content becomes one column, metrics become two columns and details become two columns. At (700px) and below, navigation becomes a (250px) off-canvas menu with a dismissible scrim. Workspace left margin disappears, top-bar height becomes (62px) and main padding becomes (24px 16px 40px). Page headings stack, forms become one column, and detail fields remain in two columns. Mobile panels use (20px 16px) padding. Login changes from two columns to one.

## Elevation & Depth

The interface is flat: white surfaces, tonal changes and fine borders establish structure. There are no decorative panel shadows. A translucent dark forest scrim separates the open mobile menu from the page, with sidebar and scrim at stacking levels (30) and (25).

### Shadow Vocabulary
- **Field focus:** (0 0 0 3px #2166501a) supplements the green focused-field border as an interaction indicator.

**The Flat Surface Rule.** Use borders and tonal surfaces for ordinary containers; preserve shadow emphasis for the existing field-focus treatment.

## Shapes

Controls use gently curved control corners; navigation uses its smaller navigation radius. Ordinary panels use the panel radius and reduce to panel-mobile on compact screens. Editors retain their panel radius on mobile. Metrics share a clipped outer panel silhouette with internal rules. Status dots are circular (7px), and avatars are circular (34px). Borders are generally (1px). Native select, date and checkbox behavior comes from React-Bootstrap and the browser.

## Components

### Buttons

Actions share a control radius, (7px) icon gap, medium weight and (42px) minimum height. Primary actions use forest green with white text, deepening on hover and active. Primary outlines use a muted green border and green hover fill. Secondary outlines use gray-green text and a pale hover fill. Link actions remove horizontal padding and underlining. Pagination uses a smaller (32px) minimum height.

All interactive elements receive a copper (3px) focus outline with a (3px) offset. Busy submissions disable submit and cancel controls and display progress copy. Bootstrap supplies remaining state mechanics.

### Inputs / Fields

White fields use the control radius, field-rule border, (44px) minimum height and (14px) text. Focus adds a green border and focus ring. Visible labels pair with fields; required markers and inline validation use red. Lookup fields combine a compact search input, native select, loading text and error text. Notes and descriptions span both editor columns. Search fields reserve left padding for an icon.

### Cards / Containers

Panels use white surfaces, panel-rule borders and no decorative shadow. Desktop panel padding is (24px); editor padding is (28px). List panels remove inner padding and contain a toolbar, scrollable table and pagination row. Table headers use pale tonal fill, rows retain fine dividers and hover adds a subtle neutral tint.

### Navigation

Forest sidebar items use (14px) text with small outline icons. Hover adds a lighter forest surface; active items reverse to warm paper with dark forest semibold text. Desktop navigation scrolls independently. The mobile sidebar slides over its scrim using (0.2s ease-out) transform; reduced-motion preference removes animations and transitions.

### Status Indicators

Quiet (12px) labels are unwrapped and preceded by a circular (7px) dot with a (6px) gap. Unmapped states retain neutral sage. There is no filled status-pill component.

### Metrics and Charts

Clickable financial metrics share one bordered surface, with pale green hover treatment and tabular numerals. Charts use forest revenue and amber expense series. Donuts, straight monthly lines and compact grouped bars provide comparisons; inline chart palettes are local configuration, not a shared semantic status scale.

### Feedback and Print

Loading states pair a spinner with Thai status text. API errors use Bootstrap danger alerts, with retry where supplied by the caller. Empty states use concise text and available actions. Success alerts are dismissible. Receipt printing uses browser print styles that remove navigation, top bar, footer and action buttons and flatten panel borders.

## Do's and Don'ts

### Do:
- **Do** retain Thai labels, visible field labels and the established font pairing.
- **Do** accompany status colors with readable text.
- **Do** keep financial figures tabular and operational surfaces flat.
- **Do** preserve keyboard focus, reduced-motion behavior and horizontally scrollable tables.
- **Do** reuse the shared panel, field and button patterns before adding visual variants.

### Don't:
- **Don't** invent business figures or imply unavailable payment integrations.
- **Don't** add a currency symbol until the product establishes the currency.
- **Don't** replace quiet status dots with color-only signals.
- **Don't** add decorative elevation to ordinary working panels.
- **Don't** treat chart-specific color arrays as a universal mapping of room status to color.

