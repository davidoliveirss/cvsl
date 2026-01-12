<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useQuasar } from 'quasar';
import { produtosService } from '@/services/produtosService';
import { authService } from '@/services/authService';
import type { Produto } from '@/types/produtos';

const $q = useQuasar();

const produtos = ref<Produto[]>([]);
const loading = ref(false);

// filtros
const search = ref('');
const categoriaSelecionada = ref<string | null>(null);
const soCriticos = ref(false);

// diálogo de movimento
const movDialog = ref(false);
const tipoMovimento = ref<'entrada' | 'saida'>('entrada');
const produtoSelecionado = ref<Produto | null>(null);
const movQuantidade = ref<number | null>(null);
const movMotivo = ref('');

// TODO: se tiveres categorias no backend, podes carregar via serviço
const categorias = ref<{ label: string; value: string }[]>([]);

const clinicaId = computed(() => {
  const token = authService.getToken();
  if (!token) return 1;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return parseInt(payload.clinicaid || payload.nameid || payload.sub || '1');
  } catch {
    return 1;
  }
});

// podes ajustar estes limites consoante o teu modelo
function stockMinimo(p: Produto) {
  return 5;
}
function stockAviso(p: Produto) {
  return 10;
}
function stockMaximo(p: Produto) {
  return 50;
}

const produtosFiltrados = computed(() => {
  return produtos.value.filter(p => {
    const matchSearch =
      !search.value ||
      p.nome.toLowerCase().includes(search.value.toLowerCase());

    const matchCategoria =
      !categoriaSelecionada.value ||
      String(p.idCategoria) === String(categoriaSelecionada.value);

    const critico = p.quantidadeStock <= stockMinimo(p);
    const matchCritico = !soCriticos.value || critico;

    return matchSearch && matchCategoria && matchCritico;
  });
});

const totalProdutos = computed(() => produtos.value.length);
const produtosCriticos = computed(() =>
  produtos.value.filter(p => p.quantidadeStock <= stockMinimo(p))
);
const valorTotalStock = computed(() =>
  produtos.value.reduce((acc, p) => acc + (p.preco || 0) * (p.quantidadeStock || 0), 0)
);

const colunas = [
  { id: 'critico', label: 'Crítico' },
  { id: 'baixo', label: 'Baixo' },
  { id: 'ok', label: 'Ok' },
  { id: 'excesso', label: 'Excesso' }
] as const;

type ColunaId = (typeof colunas)[number]['id'];

const produtosPorColuna = computed<Record<ColunaId, Produto[]>>(() => {
  const map: Record<ColunaId, Produto[]> = {
    critico: [],
    baixo: [],
    ok: [],
    excesso: []
  };

  produtosFiltrados.value.forEach(p => {
    const q = p.quantidadeStock || 0;
    let col: ColunaId = 'ok';

    if (q <= stockMinimo(p)) col = 'critico';
    else if (q <= stockAviso(p)) col = 'baixo';
    else if (q >= stockMaximo(p)) col = 'excesso';

    map[col].push(p);
  });

  return map;
});

function getStockColor(p: Produto) {
  const q = p.quantidadeStock || 0;
  if (q <= stockMinimo(p)) return 'negative';
  if (q <= stockAviso(p)) return 'warning';
  return 'positive';
}

async function loadProdutos() {
  loading.value = true;
  try {
    produtos.value = await produtosService.getAll(false);
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao carregar produtos'
    });
  } finally {
    loading.value = false;
  }
}

function openMovimentoDialog(p: Produto, tipo: 'entrada' | 'saida') {
  produtoSelecionado.value = p;
  tipoMovimento.value = tipo;
  movQuantidade.value = null;
  movMotivo.value = '';
  movDialog.value = true;
}

async function registarMovimento() {
  if (!produtoSelecionado.value || !movQuantidade.value || movQuantidade.value <= 0) {
    $q.notify({
      type: 'warning',
      message: 'Indique uma quantidade válida'
    });
    return;
  }

  try {
    const sinal = tipoMovimento.value === 'entrada' ? 1 : -1;
    const novaQuantidade =
      (produtoSelecionado.value.quantidadeStock || 0) + sinal * movQuantidade.value;

    await produtosService.update(produtoSelecionado.value.id!, {
      ...produtoSelecionado.value,
      quantidadeStock: novaQuantidade
    });

    $q.notify({
      type: 'positive',
      message: 'Movimento registado com sucesso'
    });
    movDialog.value = false;
    await loadProdutos();
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao registar movimento'
    });
  }
}

onMounted(() => {
  loadProdutos();
});
</script>

<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-md">
        <div class="col">
          <div class="text-h5">Stock</div>
          <div class="text-subtitle2 text-grey-7">
            Gestão de stock da clínica
          </div>
        </div>
      </div>

      <!-- Dashboard topo -->
      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-xs-12 col-sm-6 col-md-4">
          <q-card class="bg-primary text-white">
            <q-card-section>
              <div class="text-caption">Produtos em stock</div>
              <div class="text-h5">{{ totalProdutos }}</div>
            </q-card-section>
          </q-card>
        </div>

        <div class="col-xs-12 col-sm-6 col-md-4">
          <q-card class="bg-negative text-white">
            <q-card-section>
              <div class="text-caption">Abaixo do mínimo</div>
              <div class="text-h5">{{ produtosCriticos.length }}</div>
            </q-card-section>
          </q-card>
        </div>

        <div class="col-xs-12 col-sm-6 col-md-4">
          <q-card class="bg-secondary text-white">
            <q-card-section>
              <div class="text-caption">Valor total stock</div>
              <div class="text-h6">
                {{ valorTotalStock.toFixed(2) }} €
              </div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <!-- Filtros -->
      <div class="row items-center q-mb-md q-gutter-md">
        <q-input v-model="search" dense outlined placeholder="Procurar produto..." class="col-12 col-sm-4">
          <template #append>
            <q-icon name="search" />
          </template>
        </q-input>

        <q-select v-model="categoriaSelecionada" :options="categorias" dense outlined emit-value map-options
          class="col-12 col-sm-3" label="Categoria" clearable />

        <q-toggle v-model="soCriticos" label="Só abaixo do mínimo" />
      </div>

      <!-- Grid de cartões -->
      <div class="row q-col-gutter-md q-mb-xl">
        <div v-for="p in produtosFiltrados" :key="p.id" class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
          <q-card>
            <q-card-section>
              <div class="text-subtitle1">{{ p.nome }}</div>
              <div class="text-caption text-grey-7">
                Categoria: {{ p.idCategoria }}
              </div>
            </q-card-section>

            <q-card-section>
              <div class="row items-center justify-between q-mb-sm">
                <span>Stock: {{ p.quantidadeStock }}</span>
                <span class="text-caption">
                  / {{ p.unidadesPorCaixa }} por caixa
                </span>
              </div>

              <q-linear-progress :value="Math.min(
                1,
                (p.quantidadeStock || 0) / stockMaximo(p)
              )
                " :color="getStockColor(p)" track-color="grey-3" rounded size="10px" />

              <div class="row justify-between q-mt-xs text-caption">
                <span>Mín: {{ stockMinimo(p) }}</span>
                <span>Aviso: {{ stockAviso(p) }}</span>
              </div>
            </q-card-section>

            <q-separator />

            <q-card-actions align="around">
              <q-btn flat icon="add" label="Entrada" @click="openMovimentoDialog(p, 'entrada')" />
              <q-btn flat icon="remove" label="Saída" color="negative" @click="openMovimentoDialog(p, 'saida')" />
            </q-card-actions>
          </q-card>
        </div>

        <div v-if="!loading && produtosFiltrados.length === 0" class="col-12 text-center text-grey-6 q-mt-lg">
          Nenhum produto encontrado.
        </div>
      </div>
    </div>

    <!-- Dialog de movimentos -->
    <q-dialog v-model="movDialog" persistent>
      <q-card style="min-width: 350px">
        <q-card-section>
          <div class="text-h6">
            {{ tipoMovimento === 'entrada'
              ? 'Entrada em stock'
              : 'Saída de stock' }}
          </div>
          <div class="text-subtitle2 text-grey-7">
            {{ produtoSelecionado?.nome }}
          </div>
        </q-card-section>

        <q-card-section>
          <q-input v-model.number="movQuantidade" type="number" min="1" label="Quantidade" outlined dense
            class="q-mb-sm" />
          <q-input v-model="movMotivo" type="textarea" label="Motivo (opcional)" outlined dense />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Guardar" @click="registarMovimento" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>