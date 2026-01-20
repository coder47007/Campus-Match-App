# Component Size Audit - Phase 3.3

## Large Components Found (>300 Lines)

| Component | Lines | Status | Priority |
|-----------|-------|--------|----------|
| `profile.tsx` | **1093** | 🔴 CRITICAL | Must refactor |
| `discover.tsx` | 700 | 🔴 HIGH | Must refactor |
| `explore.tsx` | 461 | 🟡 MEDIUM | Should refactor |
| `matches.tsx` | 381 | 🟡 MEDIUM | Should refactor |
| `likes.tsx` | 276 | ✅ OK | - |
| `_layout.tsx` | 138 | ✅ OK | - |

---

## Refactoring Plan

### Priority 1: profile.tsx (1093 lines!)

**Current Structure**: Monolithic file with everything
**Target**: < 300 lines per component

**Components to Extract**:
```
profile.tsx (main, ~150 lines)
├── components/
│   ├── ProfileHeader.tsx (~100 lines)
│   │   - Avatar, name, age, university
│ │   - Edit button, settings button
│   │
│   ├── ProfilePhotos.tsx (~150 lines)
│   │   - Photo grid
│   │   - Add/delete photos
│   │   - Drag to reorder
│   │
│   ├── ProfilePrompts.tsx (~120 lines)
│   │   - Prompt cards
│   │   - Edit/add prompts
│   │
│   ├── ProfileInterests.tsx (~80 lines)
│   │   - Interest chips
│   │   - Add/remove interests
│   │
│   ├── ProfileBasicInfo.tsx (~100 lines)
│   │   - Bio
│   │   - Major, Year
│   │   - Instagram
│   │
│   └── ProfilePreferences.tsx (~150 lines)
│       - Age range
│       - Gender preferences
│       - Distance settings
│
└── styles/
    └── profile.styles.ts (~100 lines)
```

### Priority 2: discover.tsx (700 lines)

**Components to Extract**:
```
discover.tsx (main, ~200 lines)
├── components/
│   ├── DiscoverHeader.tsx (~80 lines)
│   │   - Logo
│   │   - Filter button
│   │   - Study buddy toggle
│   │
│   ├── SwipeCardStack.tsx (~200 lines)
│   │   - Card rendering logic
│   │   - Swipe handling
│   │
│   ├── SwipeActionButtons.tsx (~100 lines)
│   │   - Pass/Like/SuperLike buttons
│   │   - Rewind button
│   │
│   ├── DiscoverEmptyState.tsx (~50 lines)
│   │   - No profiles message
│   │
│   └── FilterModal.tsx (~150 lines)
│       - Age sliders
│       - Gender selection
│       - Major/Year filters
│
└── styles/
    └── discover.styles.ts (~100 lines)
```

### Priority 3: explore.tsx (461 lines)

**Components to Extract**:
```
explore.tsx (main, ~150 lines)
├── components/
│   ├── TrendingSpots.tsx (~120 lines)
│   ├── EventCard.tsx (~100 lines)
│   └── CheckInButton.tsx (~60 lines)
│
└── styles/
    └── explore.styles.ts (~50 lines)
```

---

## Benefits of Refactoring

✅ **Testability**: Smaller components are easier to test  
✅ **Reusability**: Components can be used in multiple places  
✅ **Maintainability**: Easier to find and fix bugs  
✅ **Performance**: Can use React.memo() on smaller components  
✅ **Team Collaboration**: Multiple developers can work on different components  

---

## Estimated Effort

| Component | Extraction Time | Testing Time | Total |
|-----------|----------------|--------------|-------|
| profile.tsx | 3-4 hours | 1-2 hours | ~5 hours |
| discover.tsx | 2-3 hours | 1 hour | ~3 hours |
| explore.tsx | 1-2 hours | 30 min | ~2 hours |
| matches.tsx | 1-2 hours | 30 min | ~2 hours |

**Total**: ~12 hours for complete refactor

---

## Next Steps

1. Start with `profile.tsx` (biggest impact)
2. Extract `ProfileHeader` first (most reusable)
3. Test each extraction before moving to next
4. Apply same pattern to other large components
