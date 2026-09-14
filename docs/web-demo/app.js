const state = {
  maps: [],
  selectedName: 'SevenSegment',
  currentChar: 'A'
};

const select = document.getElementById('mapSelect');
const charInput = document.getElementById('charInput');
const panel = document.getElementById('panel');

function normalizeMapName(name) {
  return String(name || '').trim();
}

function parseHexMask(value) {
  if (value == null || value === 'null') {
    return 0n;
  }

  const trimmed = String(value).trim();
  if (!trimmed) {
    return 0n;
  }

  const normalized = trimmed.startsWith('0x') || trimmed.startsWith('0X') ? trimmed.slice(2) : trimmed;
  return BigInt(`0x${normalized}`);
}

function getSupportedCharacters(map) {
  if (!map || !map.Characters) {
    return [];
  }

  return Object.keys(map.Characters).filter((key) => key && key.length === 1 && map.Characters[key] !== 'null');
}

function chooseCharacter(map, preferred) {
  const names = getSupportedCharacters(map);
  if (!names.length) {
    return 'A';
  }

  if (names.includes(preferred)) {
    return preferred;
  }

  return names[0];
}

function normalizeDisplayText(map, value) {
  const text = String(value || '').trim() || 'A';
  if (!map || !map.Characters) {
    return text;
  }

  return [...text].map((character) => {
    if (character in map.Characters) {
      return character;
    }

    if (character === '−' || character === '–' || character === '—') {
      return '-';
    }

    if (character === '·' || character === '•') {
      return '.';
    }

    if (character === '：') {
      return ':';
    }

    const token = character === '.' || character === ':' || character === '-' ? character : null;
    return token && token in map.Characters ? token : character;
  }).join('');
}

function renderBitmap(map, text) {
  const glyphs = [...String(text || 'A')].map((character) => {
    const normalized = normalizeDisplayText(map, character);
    const bitmapText = map.Characters[normalized] || map.Characters['A'] || '0';
    const rows = String(bitmapText).split('|').filter(Boolean);
    const width = rows[0]?.length || 0;
    return { normalized, rows, width };
  });

  const totalWidth = glyphs.reduce((sum, glyph) => sum + glyph.width, 0) + Math.max(0, glyphs.length - 1) * 8;
  const gridStyle = `grid-template-columns: repeat(${Math.max(...glyphs.map((glyph) => glyph.width), 1)}, 18px);`;

  const cells = glyphs.flatMap((glyph, glyphIndex) => {
    const offset = glyphIndex === 0 ? 0 : glyphs.slice(0, glyphIndex).reduce((sum, item) => sum + item.width, 0) + glyphIndex * 8;
    return glyph.rows.flatMap((row, rowIndex) =>
      Array.from(row, (cell, colIndex) => {
        const active = cell === '1';
        return `<span class="bitmap-cell ${active ? 'on' : ''}" title="${glyph.normalized} @ ${rowIndex},${colIndex} + ${offset}"></span>`;
      })
    );
  }).join('');

  return `
    <div class="meta">
      <h2>${map.Name}</h2>
      <span class="badge">Bitmap</span>
    </div>
    <div class="display-wrap">
      <div class="bitmap-grid" style="${gridStyle}">${cells}</div>
    </div>
    <div class="glyph-list">
      ${getSupportedCharacters(map).slice(0, 12).map((key) => `<span class="glyph-pill">${key}</span>`).join('')}
    </div>
  `;
}

function renderSegmentDisplay(map, text) {
  const layout = map.Layout || { CanvasWidth: 220, CanvasHeight: 260, Segments: [] };
  const baseWidth = layout.CanvasWidth || 220;
  const height = layout.CanvasHeight || 260;
  const points = layout.Segments || [];
  const glyphs = [...String(text || 'A')].map((character, index) => {
    const normalized = normalizeDisplayText(map, character);
    const maskValue = parseHexMask(map.Characters[normalized] ?? map.Characters['A'] ?? '0x0');
    const segments = points.map((segmentPoints, segmentIndex) => {
      const isActive = (maskValue & (1n << BigInt(segmentIndex))) !== 0n;
      const pointsText = segmentPoints.map(([x, y]) => `${x},${y}`).join(' ');
      return `<polygon class="segment ${isActive ? 'on' : 'off'}" points="${pointsText}" transform="translate(${index * (baseWidth + 12)}, 0)"></polygon>`;
    }).join('');
    return segments;
  }).join('');

  const totalWidth = Math.max(1, (glyphs.length ? String(text || 'A').length : 1) * (baseWidth + 12));

  return `
    <div class="meta">
      <h2>${map.Name}</h2>
      <span class="badge">Segmented</span>
    </div>
    <div class="display-wrap">
      <svg viewBox="0 0 ${totalWidth} ${height}" role="img" aria-label="${text} glyph for ${map.Name}">
        ${glyphs}
      </svg>
    </div>
    <div class="glyph-list">
      ${getSupportedCharacters(map).slice(0, 12).map((key) => `<span class="glyph-pill">${key}</span>`).join('')}
    </div>
  `;
}

function renderSelectedPanel() {
  const map = state.maps.find((entry) => entry[0] === state.selectedName)?.[1];
  if (!map) {
    panel.innerHTML = '<p>No map selected.</p>';
    return;
  }

  const text = normalizeDisplayText(map, state.currentChar || 'A');
  state.currentChar = text;
  charInput.value = text;

  const isBitmap = Number(map.Kind) === 1;
  panel.innerHTML = isBitmap ? renderBitmap(map, text) : renderSegmentDisplay(map, text);
}

async function loadMaps() {
  const response = await fetch('./maps.json');
  if (!response.ok) {
    throw new Error(`Unable to read maps catalog (${response.status})`);
  }

  const payload = await response.json();
  const entries = Object.entries(payload).sort(([a], [b]) => a.localeCompare(b));
  state.maps = entries;

  select.innerHTML = '';
  entries.forEach(([name]) => {
    const option = document.createElement('option');
    option.value = name;
    option.textContent = name;
    select.appendChild(option);
  });

  select.value = state.selectedName;
  charInput.value = state.currentChar;
  renderSelectedPanel();
}

select.addEventListener('change', (event) => {
  state.selectedName = normalizeMapName(event.target.value);
  const map = state.maps.find(([name]) => name === state.selectedName)?.[1];
  const fallback = map ? chooseCharacter(map, String(state.currentChar || 'A')) : 'A';
  state.currentChar = fallback;
  renderSelectedPanel();
});

charInput.addEventListener('input', (event) => {
  const value = event.target.value?.trim() || 'A';
  const map = state.maps.find(([name]) => name === state.selectedName)?.[1];
  if (!map) {
    return;
  }

  state.currentChar = value;
  renderSelectedPanel();
});

loadMaps().catch((error) => {
  panel.innerHTML = `<p>Unable to load glyph catalog: ${error.message}</p>`;
});
