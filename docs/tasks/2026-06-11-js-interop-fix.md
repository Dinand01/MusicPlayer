# Task: Fix JavaScript Interop for CefGlue.Avalonia

**Date:** 2026-06-11  
**Status:** in_progress  
**Agent:** MusicPlayer-Maintainer

---

## Overview

Fix the broken JavaScript interop between React frontend and C# backend. The migration from CefSharp.Wpf to CefGlue.Avalonia was incomplete - the `window.external` pattern doesn't work in CefGlue, and the native object binding system was never connected to `MusicPlayerGate`.

---

## Problem Analysis

### Root Cause

1. **`window.external` pattern broken**: `MainWindow.xaml.cs:67-94` injects JS that calls `window.external.OpenFolder()` etc.
   - CefGlue.Avalonia does NOT support `window.external` (CefSharp.Wpf-specific feature)
   - This results in `undefined` being returned, causing `undefined.then()` error

2. **Missing Promise returns**: `openFolder()` and `openFiles()` don't return Promises
   - JavaScript in `Home.jsx:25` calls `MusicPlayer.openFolder().then(...)`
   - Returns `undefined` because the function doesn't return anything

3. **Native object binding not connected**: The CefGlue binding system exists but `MusicPlayerGate` is never registered
   - `JavascriptToNativeDispatcherRenderSide` handles native object registration
   - No code exists to register `MusicPlayerGate` as a native JS object

---

## Solution Options

### Option A: Use CefGlue's Native Object Binding (Recommended)
- Register `MusicPlayerGate` as a native object via the existing binding system
- Methods return Promises automatically via `V8FunctionHandler`
- Uses `cefglue.Bind("MusicPlayerGate")` pattern from `CefGlueGlobalScript.js`

### Option B: Direct JS Injection Only
- Keep current `ExecuteJavaScript` approach for all method calls
- Manually wrap all methods in Promises
- Requires updating all JS interop code

---

## Implementation Plan

### Step 1: Fix JavaScript interop in MainWindow.xaml.cs
- Remove `window.external` pattern
- Inject correct `window.MusicPlayer` object using CefGlue binding system

### Step 2: Register MusicPlayerGate as native JS object
- Create method to register `MusicPlayerGate` with `JavascriptToNativeDispatcherRenderSide`
- Connect the renderer process handler to the browser process

### Step 3: Update openFolder() and openFiles() to return Promises
- Modify `MusicPlayerGate.Actions.cs` to return `bool` as JSON string
- Wrap in Promise-compatible format

### Step 4: Update CefGlue binding to handle async dialog methods
- Ensure `OpenFolder()` and `OpenFiles()` can be called asynchronously
- Handle UI thread marshaling correctly

### Step 5: Fix index.html test code
- Update to use correct Promise patterns for testing

### Step 6: Build and test
- Build the application
- Verify JS interop works at runtime

### Step 7: Update documentation
- Update ARCHITECTURE.md
- Update HOW-IT-WORKS.md
- Update API.md

---

## Files to Modify

| File | Change |
|------|--------|
| `MusicPlayerWeb/MainWindow.xaml.cs` | Remove window.external injection, use native binding |
| `MusicPlayerWeb/MusicPlayerGate.Actions.cs` | Fix return types for JS interop |
| `MusicPlayerWeb/MusicPlayerGate.cs` | Add native object registration |
| `MusicPlayerWeb/Program.cs` | Ensure proper CEF initialization |
| `MusicPlayerWeb/web/Pages/index.html` | Fix test code |
| `docs/API.md` | Update JS interop documentation |
| `docs/HOW-IT-WORKS.md` | Update flow diagrams |
| `docs/ARCHITECTURE.md` | Update architecture notes |

---