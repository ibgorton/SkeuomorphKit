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

function renderBitmap(map, character) {
  const bitmapText = map.Characters[character] || '0';
  const rows = String(bitmapText).split('|').filter(Boolean);
  const width = rows[0]?.length || 0;
  const gridStyle = `grid-template-columns: repeat(${width}, 18px);`;

  const cells = rows.flatMap((row, rowIndex) =>
    Array.from(row, (cell, colIndex) => {
      const active = cell === '1';
      return `<span class="bitmap-cell ${active ? 'on' : ''}" title="${rowIndex},${colIndex}"></span>`;
    })
  ).join('');

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

function renderSegmentDisplay(map, character) {
  const layout = map.Layout || { CanvasWidth: 220, CanvasHeight: 260, Segments: [] };
  const width = layout.CanvasWidth || 220;
  const height = layout.CanvasHeight || 260;
  const points = layout.Segments || [];
  const maskValue = parseHexMask(map.Characters[character]);

  const segments = points.map((segmentPoints, index) => {
    const isActive = (maskValue & (1n << BigInt(index))) !== 0n;
    const pointsText = segmentPoints.map(([x, y]) => `${x},${y}`).join(' ');
    return `<polygon class="segment ${isActive ? 'on' : 'off'}" points="${pointsText}"></polygon>`;
  }).join('');

  return `
    <div class="meta">
      <h2>${map.Name}</h2>
      <span class="badge">Segmented</span>
    </div>
    <div class="display-wrap">
      <svg viewBox="0 0 ${width} ${height}" role="img" aria-label="${character} glyph for ${map.Name}">
        ${segments}
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

  const char = chooseCharacter(map, state.currentChar);
  state.currentChar = char;
  charInput.value = char;

  const isBitmap = Number(map.Kind) === 1;
  panel.innerHTML = isBitmap ? renderBitmap(map, char) : renderSegmentDisplay(map, char);
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
  state.currentChar = map ? chooseCharacter(map, state.currentChar) : 'A';
  renderSelectedPanel();
});

charInput.addEventListener('input', (event) => {
  const value = event.target.value?.trim() || 'A';
  const map = state.maps.find(([name]) => name === state.selectedName)?.[1];
  if (!map) {
    return;
  }

  const next = value.length ? value[0] : 'A';
  state.currentChar = next;
  renderSelectedPanel();
});

loadMaps().catch((error) => {
  panel.innerHTML = `<p>Unable to load glyph catalog: ${error.message}</p>`;
});
